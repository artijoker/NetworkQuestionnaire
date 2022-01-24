using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Library;
using System.Net.Sockets;
using System.Collections.ObjectModel;
using System.Net;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace Server {
    public partial class MainWindow : Window {
        private TcpClient? _admin;
        private IList<ConnectedEmployee> _employees;
        private bool _isAdminConnect;
        public ObservableCollection<string> Logs { get; }

        public MainWindow() {
            InitializeComponent();
            DataContext = this;

            Logs = new ObservableCollection<string>();
            _employees = new List<ConnectedEmployee>();
            _isAdminConnect = false;
        }


        private void Window_Loaded(object sender, RoutedEventArgs e) {
            IConfiguration config = Host.CreateDefaultBuilder().Build().Services.GetRequiredService<IConfiguration>();
            string ipAddress = config.GetValue<string>("IpAddress");
            int port = config.GetValue<int>("Port");

            TcpListener listener = new TcpListener(IPAddress.Parse(ipAddress), port);
            Title = $"IP = {ipAddress} Port = {port}";
            listener.Start();
            ServeClients(listener);
        }

        private async void ServeClients(TcpListener listener) {
            while (true) {
                TcpClient client = await listener.AcceptTcpClientAsync();
                ServeClient(client);
            }
        }

        private async void ServeClient(TcpClient client) {

            try {
                while (true) {
                    byte[] buffer = await client.ReadFromStream(1);
                    byte message = buffer[0];

                    if (message == Message.Authorization) {
                        Employee? employee = await GetEmployee(client);
                        if (employee is null) {
                            string errorMessage = "Ошибка! Неверный логин или пароль";
                            await SendMessageClient.SendAuthorizationFailedMessage(client, errorMessage);
                            Logs.Add($"Пользователь {client.Client.RemoteEndPoint} ошибка при авторизации. Неверный логин или пароль.");
                        }
                        else if (_employees.Any(e => e.Employee.Id == employee.Id)) {
                            string errorMessage = "Ошибка! Этот сотрудник уже сети.";
                            await SendMessageClient.SendAuthorizationFailedMessage(client, errorMessage);
                            Logs.Add($"Пользователь {client.Client.RemoteEndPoint} ошибка при авторизации. ");
                        }
                        else {
                            await SendMessageClient.SendAuthorizationSuccessfulMessage(client, employee);
                            Logs.Add($"Пользователь {client.Client.RemoteEndPoint} прошел авторизацию.");
                            lock (_employees)
                                _employees.Add(new(employee, client));
                        }
                    }
                    else if (message == Message.AdminConnect) {
                        if (!_isAdminConnect) {
                            lock (_employees)
                                _admin = client;
                            _isAdminConnect = true;
                            await SendMessageClient.SendAdminConnectSuccessfulMessage(client);
                            Dispatcher.Invoke(() => Logs.Add($"Администратор подключился {client.Client.RemoteEndPoint} {DateTime.Now}."));
                        }
                        else {
                            await SendMessageClient.SendAdminConnectFailedMessage(client, "Ошибка! Администратор уже в сети.");
                            if (client.Client.Connected)
                                client.Client.Shutdown(SocketShutdown.Both);
                            client.Client.Close();
                            return;
                        }
                    }
                    else if (message == Message.ListSurveysNotСompletedEmployee) {
                        buffer = await client.ReadFromStream(4);
                        int employeeId = BitConverter.ToInt32(buffer, 0);
                        Survey[] surveys;
                        using (SurveysContext context = new()) {
                            surveys = context.Surveys.Where(survey => !survey.Employees.Any(employee => employee.Id == employeeId))
                                .Include(s => s.Questions)
                                    .ThenInclude(q => q.Type)
                                .Include(s => s.Questions)
                                    .ThenInclude(q => q.SingleAnswers)
                                .Include(s => s.Questions)
                                    .ThenInclude(q => q.MultipleAnswers)
                                .Include(s => s.Questions)
                                    .ThenInclude(q => q.FreeAnswers)
                               .ToArray();
                        }
                        await SendMessageClient.SendSurveyListMessage(client, surveys);
                    }
                    else if (message == Message.AllSurveyFromDBAndQuestionTypes) {
                        Survey[] surveys;
                        QuestionType[] questionTypes;
                        using (SurveysContext context = new()) {
                            context.QuestionTypes.Load();
                            context.Surveys.Load();
                            context.Questions.Load();
                            context.FreeAnswers.Load();
                            context.MultipleAnswers.Load();
                            context.SingleAnswers.Load();
                            surveys = context.Surveys.ToArray();
                            questionTypes = context.QuestionTypes.ToArray();
                        }

                        await SendMessageClient.SendAllSurveyAndQuestionTypeMessage(client, surveys, questionTypes);
                    }

                    else if (message == Message.EmployeeAnswers) {
                        buffer = await client.ReadFromStream(4);
                        buffer = await client.ReadFromStream(BitConverter.ToInt32(buffer, 0));

                        EmployeeSurveyAnswer employeeAnswer = JsonSerializer.Deserialize<EmployeeSurveyAnswer>(
                            Encoding.UTF8.GetString(buffer), options: new() { IncludeFields = true });
                        SaveEmployeeAnswers(employeeAnswer);

                        await SendMessageClient.SendDataSaveSuccessMessage(client, "Данные успешно сохранены!");
                        Logs.Add($"Пользователь {client.Client.RemoteEndPoint} прошел опрос.");
                    }

                    else if (message == Message.EmployeesList) {

                        Employee[] employees;
                        using (SurveysContext context = new()) {
                            employees = context.Employees.ToArray();
                        }
                        await SendMessageClient.SendEmployeeListMessage(client, employees);
                    }
                    else if (message == Message.AddNewEmployee) {
                        buffer = await client.ReadFromStream(4);
                        buffer = await client.ReadFromStream(BitConverter.ToInt32(buffer, 0));

                        Employee employee = Employee.FromDTO(JsonSerializer.Deserialize<EmployeeDTO>(Encoding.UTF8.GetString(buffer)));

                        using (SurveysContext context = new()) {
                            context.Employees.Add(employee);
                            context.SaveChanges();
                        }
                        await SendMessageClient.SendDataSaveSuccessMessage(client, "Новый сотрудник добавлен в базу данных!");
                        Logs.Add($"В базу добавлен новый сотрудник.");
                    }
                    else if (message == Message.EditEmployee) {
                        buffer = await client.ReadFromStream(4);
                        buffer = await client.ReadFromStream(BitConverter.ToInt32(buffer, 0));

                        Employee employee = Employee.FromDTO(JsonSerializer.Deserialize<EmployeeDTO>(Encoding.UTF8.GetString(buffer)));

                        using (SurveysContext context = new()) {
                            context.Update(employee);
                            context.SaveChanges();
                        }
                        await SendMessageClient.SendDataSaveSuccessMessage(client, "Измененые данные о сотруднике зафиксированы в базе данных!");
                        Logs.Add($"Измененые данные о сотруднике зафиксированы в базе.");
                    }
                    else if (message == Message.RemoveEmployee) {
                        buffer = await client.ReadFromStream(4);
                        buffer = await client.ReadFromStream(BitConverter.ToInt32(buffer, 0));

                        Employee employee = Employee.FromDTO(JsonSerializer.Deserialize<EmployeeDTO>(Encoding.UTF8.GetString(buffer)));

                        using (SurveysContext context = new()) {
                            context.Employees.Remove(employee);
                            context.SaveChanges();
                        }
                        await SendMessageClient.SendDataSaveSuccessMessage(client, "Сотрудник удален из базы данных!");
                        Logs.Add($"Из базы удален сотрудник.");
                    }
                    else if (message == Message.SurveysСompletedEmployee) {
                        buffer = await client.ReadFromStream(4);
                        int employeeId = BitConverter.ToInt32(buffer, 0);
                        Survey[] surveys = GetSurveysСompletedEmployee(employeeId);
                        await SendMessageClient.SendAllAnswersEmployeeMessage(client, surveys);
                    }
                    else if (message == Message.AddNewSurvey) {
                        buffer = await client.ReadFromStream(4);
                        buffer = await client.ReadFromStream(BitConverter.ToInt32(buffer, 0));

                        Survey survey = Survey.FromDTO(JsonSerializer.Deserialize<SurveyDTO>(Encoding.UTF8.GetString(buffer)));
                        foreach (var question in survey.Questions)
                            question.Type = null;
                        using (SurveysContext context = new()) {
                            context.Surveys.Add(survey);
                            context.SaveChanges();
                        }
                        await SendMessageClient.SendDataSaveSuccessMessage(client, "Новый опрос добавлен в базу данных!");
                        Logs.Add($"В базу добавлен новый опрос.");
                    }
                    else if (message == Message.EditSurvey) {
                        buffer = await client.ReadFromStream(4);
                        buffer = await client.ReadFromStream(BitConverter.ToInt32(buffer, 0));

                        Survey survey = Survey.FromDTO(JsonSerializer.Deserialize<SurveyDTO>(Encoding.UTF8.GetString(buffer)));
                        foreach (var question in survey.Questions)
                            question.Type = null;
                        UpdateSurvey(survey);

                        await SendMessageClient.SendDataSaveSuccessMessage(client, "Измененые опрос зафиксирован в базе данных!");
                        Logs.Add($"Измененые опрос зафиксирован в базе.");
                    }
                    else if (message == Message.RemoveSurvey) {
                        buffer = await client.ReadFromStream(4);
                        int surveyId = BitConverter.ToInt32(buffer, 0);

                        Survey survey = new Survey() { Id = surveyId };
                        using (SurveysContext context = new()) {
                            context.Surveys.Remove(survey);
                            context.SaveChanges();
                        }
                        await SendMessageClient.SendDataSaveSuccessMessage(client, "Опрос удален из базы данных!");
                        Logs.Add($"Из базы удален опрос.");
                    }

                }
            }
            catch (Exception) {
                ConnectedEmployee? connectedEmployee =
                    _employees.Where(employee => employee.Client.Client.RemoteEndPoint == client.Client.RemoteEndPoint)
                    .SingleOrDefault();
                if (_admin is not null && _admin.Client.RemoteEndPoint == client.Client.RemoteEndPoint)
                    _isAdminConnect = false;
                
                Logs.Add($"Отключился {client.Client.RemoteEndPoint} {DateTime.Now}.");
                if (client.Client.Connected)
                    client.Client.Shutdown(SocketShutdown.Both);
                client.Client.Close();
                if (connectedEmployee is not null)
                    lock (_employees) {
                        _employees.Remove(connectedEmployee);
                    }


            }
        }
        private async Task<Employee?> GetEmployee(TcpClient client) {
            byte[] buffer = await client.ReadFromStream(4);
            byte[] key = await client.ReadFromStream(BitConverter.ToInt32(buffer, 0));

            buffer = await client.ReadFromStream(4);
            buffer = await client.ReadFromStream(BitConverter.ToInt32(buffer, 0));
            string base64 = Encoding.UTF8.GetString(buffer);

            string login = Encryption.Decrypt(base64, key);


            buffer = await client.ReadFromStream(4);
            key = await client.ReadFromStream(BitConverter.ToInt32(buffer, 0));

            buffer = await client.ReadFromStream(4);
            buffer = await client.ReadFromStream(BitConverter.ToInt32(buffer, 0));
            base64 = Encoding.UTF8.GetString(buffer);

            string password = Encryption.Decrypt(base64, key);

            Employee? employee;
            using (SurveysContext context = new()) {
                employee = context.Employees.Where(employee => employee.Login == login && employee.Password == password).SingleOrDefault();
            }
            return employee;

        }

        private void SaveEmployeeAnswers(EmployeeSurveyAnswer employeeAnswer) {
            Employee employee = new() { Id = employeeAnswer.EmployeeId };


            Survey survey = new() { Id = employeeAnswer.SurveyId };

            using (SurveysContext context = new()) {
                context.Attach(employee);
                context.Attach(survey);
                employee.Surveys.Add(survey);
                survey.Employees.Add(employee);

                foreach (var tuple in employeeAnswer.FreeAnswersIds) {
                    int id = tuple.Item1;
                    string text = tuple.Item2;

                    FreeAnswer freeAnswer = new() { Id = id };
                    context.Attach(freeAnswer);

                    EmployeeFreeAnswer employeeFreeAnswer = new EmployeeFreeAnswer() {
                        FreeAnswer = freeAnswer,
                        Employee = employee,
                        Text = text
                    };
                    freeAnswer.EmployeeFreeAnswers.Add(employeeFreeAnswer);
                    employee.EmployeeFreeAnswers.Add(employeeFreeAnswer);

                }
                foreach (var id in employeeAnswer.SingleAnswersIds) {
                    SingleAnswer singleAnswer = new() { Id = id };

                    context.Attach(singleAnswer);
                    employee.SingleAnswers.Add(singleAnswer);
                    singleAnswer.Employees.Add(employee);

                }
                foreach (var id in employeeAnswer.MultipleAnswersIds) {
                    MultipleAnswer multipleAnswer = new() { Id = id };

                    context.Attach(multipleAnswer);
                    employee.MultipleAnswers.Add(multipleAnswer);
                    multipleAnswer.Employees.Add(employee);

                }

                context.SaveChanges();
            }
        }

        private Survey[] GetSurveysСompletedEmployee(int employeeId) {

            using (SurveysContext context = new()) {
                return context.Surveys.Where(survey => survey.Employees.Any(employee => employee.Id == employeeId))
                   .Include(survey => survey.Questions)
                       .ThenInclude(question => question.Type)
                   .Include(s => s.Questions)
                       .ThenInclude(question => question.SingleAnswers)
                           .ThenInclude(a => a.Employees.Where(employee => employee.Id == employeeId))
                   .Include(survey => survey.Questions)
                       .ThenInclude(question => question.MultipleAnswers)
                           .ThenInclude(answer => answer.Employees.Where(employee => employee.Id == employeeId))
                   .Include(survey => survey.Questions)
                       .ThenInclude(question => question.FreeAnswers)
                           .ThenInclude(answer => answer.EmployeeFreeAnswers.Where(employee => employee.EmployeeId == employeeId))
                   .ToArray();
            }
        }

        private void UpdateSurvey(Survey survey) {
            using (SurveysContext context = new()) {
                Question[]? questions = context.Questions
                    .Where(question => question.SurveyId == survey.Id)
                    .ToArray();

                context.Questions.RemoveRange(
                    questions.Where(question => !survey.Questions.Any(q => q.Id == question.Id))
                    .ToArray()
                    );

                context.Questions.AddRange(
                    survey.Questions.Where(question => !context.Questions.Any(q => q.Id == question.Id))
                    .ToArray()
                    );

                questions = survey.Questions.Where(
                    question => context.Questions.Any(q => q.Id == question.Id && question.Id != 0))
                    .ToArray();

                questions.ForEach(question => UpdateQuestion(context, question));


                Survey surveyFromDB = context.Surveys.Where(s => s.Id == survey.Id).Single();
                surveyFromDB.Name = survey.Name;
                Employee[]? employees = context.Employees
                    .Include(e => e.Surveys)
                    .Where(e => e.Surveys.Any(s => s.Id == surveyFromDB.Id))
                    .ToArray();
                employees.ForEach(e => e.Surveys.Remove(surveyFromDB));
                surveyFromDB.Employees.Clear();

                context.SaveChanges();
            }
        }

        private void UpdateQuestion(SurveysContext context, Question question) {
            SingleAnswer[]? singleAnswersFromDB = context.SingleAnswers.Where(answer => answer.QuestionId == question.Id).ToArray();
            context.SingleAnswers.RemoveRange(singleAnswersFromDB.Where(answer => !question.SingleAnswers.Any(a => a.Id == answer.Id)).ToArray());
            context.SingleAnswers.AddRange(question.SingleAnswers.Where(answer => !context.SingleAnswers.Any(a => a.Id == answer.Id)).ToArray());
            singleAnswersFromDB = context.SingleAnswers.Local.Where(answer => answer.QuestionId == question.Id).ToArray();
            singleAnswersFromDB = singleAnswersFromDB.Where(answer => answer.Id != 0).ToArray();


            foreach (SingleAnswer answer in singleAnswersFromDB) {
                answer.Text = question.SingleAnswers.Where(a => a.Id == answer.Id).Single().Text;
                Employee[]? employees = context.Employees.Include(e => e.SingleAnswers).Where(e => e.SingleAnswers.Any(s => s.Id == answer.Id)).ToArray();
                employees.ForEach(e => e.SingleAnswers.Remove(answer));
                answer.Employees.Clear();
            }


            MultipleAnswer[]? multipleAnswersFromDB = context.MultipleAnswers.Where(answer => answer.QuestionId == question.Id).ToArray();
            context.MultipleAnswers.RemoveRange(multipleAnswersFromDB.Where(answer => !question.MultipleAnswers.Any(a => a.Id == answer.Id)).ToArray());

            context.MultipleAnswers.AddRange(question.MultipleAnswers.Where(answer => !context.MultipleAnswers.Any(a => a.Id == answer.Id)).ToArray());

            multipleAnswersFromDB = context.MultipleAnswers.Local.Where(answer => answer.QuestionId == question.Id).ToArray();
            multipleAnswersFromDB = multipleAnswersFromDB.Where(answer => answer.Id != 0).ToArray();

            foreach (MultipleAnswer answer in multipleAnswersFromDB) {
                answer.Text = question.MultipleAnswers.Where(a => a.Id == answer.Id).Single().Text;
                Employee[]? employees = context.Employees.Include(e => e.MultipleAnswers).Where(e => e.MultipleAnswers.Any(s => s.Id == answer.Id)).ToArray();
                employees.ForEach(e => e.MultipleAnswers.Remove(answer));
                answer.Employees.Clear();
            }


            FreeAnswer? freeAnswerFromDB = context.FreeAnswers.Where(answer => answer.QuestionId == question.Id).SingleOrDefault();
            if (freeAnswerFromDB is not null) {
                if (question.FreeAnswers.Count == 0) {
                    context.FreeAnswers.Remove(freeAnswerFromDB);
                }
                else {
                    freeAnswerFromDB.EmployeeFreeAnswers.Clear();
                    Employee[]? employees = context.Employees.Include(e => e.EmployeeFreeAnswers).Where(e => e.EmployeeFreeAnswers.Any(e => e.FreeAnswerId == freeAnswerFromDB.Id)).ToArray();
                    employees.ForEach(e => e.EmployeeFreeAnswers.Remove(context.EmployeesFreeAnswers.Where(e => e.FreeAnswerId == freeAnswerFromDB.Id).Single()));
                }
            }

            if (freeAnswerFromDB is null && question.FreeAnswers.Count == 1)
                context.FreeAnswers.Add(question.FreeAnswers.Single());

            Question questionFromDB = context.Questions.Where(q => q.Id == question.Id).Single();
            questionFromDB.IsRequired = question.IsRequired;
            questionFromDB.Text = question.Text;
            questionFromDB.QuestionTypeId = question.QuestionTypeId;
        }


    }
}
