using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using System.IO;
using Library;
using System.Net.Sockets;
using System.Collections.ObjectModel;
using System.Net;

namespace Server {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {

        private IList<TcpClient> _clients;
        public ObservableCollection<string> Logs { get; }

        public MainWindow() {
            InitializeComponent();
            DataContext = this;
            Logs = new ObservableCollection<string>();
            _clients = new List<TcpClient>();
            //ConnectDatabase();
        }

        void ConnectDatabase() {
            using (SurveysContext context = new()) {
                context.QuestionTypes.Load();
                var survey = context.Surveys.Where(s => s.Id == 1).Include("Questions").First();

                //foreach (var question in survey.Questions) {
                //    context.Entry(question).Collection("FreeAnswers").Load();
                //    context.Entry(question).Collection("MultipleAnswers").Load();
                //    context.Entry(question).Collection("SingleAnswers").Load();
                //}




                string jsonString = JsonSerializer.Serialize(survey.Questions.Select(question => question.ToDTO()).ToArray());
                File.WriteAllText("test.json", jsonString);
                var encoding = Encoding.UTF8.GetBytes(jsonString);
                var decoding = Encoding.UTF8.GetString(encoding);
                var survey1 = JsonSerializer.Deserialize<QuestionDTO[]>(decoding);

                int x = 5;
            }


        }

        private void Window_Loaded(object sender, RoutedEventArgs e) {

            TcpListener listener = new TcpListener(IPAddress.Parse("127.0.0.1"), 56537);
            Title = $"IP = {"127.0.0.1"} Port = {56537}";
            listener.Start();
            ServeClients(listener);
        }
        private async void ServeClients(TcpListener listener) {
            while (true) {
                TcpClient client = await listener.AcceptTcpClientAsync();
                lock (_clients)
                    _clients.Add(client);

                ServeClient(client);
            }
        }

        private async void ServeClient(TcpClient client) {
            Dispatcher.Invoke(() => Logs.Add($"Подключился {client.Client.RemoteEndPoint} {DateTime.Now}"));
            try {
                while (true) {
                    byte[] buffer = await client.ReadFromStream(1);
                    byte message = buffer[0];

                    if (message == Message.Authorization) {
                        buffer = await client.ReadFromStream(4);
                        buffer = await client.ReadFromStream(BitConverter.ToInt32(buffer, 0));
                        string login = Encoding.UTF8.GetString(buffer);

                        buffer = await client.ReadFromStream(4);
                        buffer = await client.ReadFromStream(BitConverter.ToInt32(buffer, 0));
                        string password = Encoding.UTF8.GetString(buffer);

                        using (SurveysContext context = new()) {
                            var employee = context.Employees.Where(employee => employee.Login == login && employee.Password == password).FirstOrDefault();
                            if (employee is null) {
                                await SendMessageClient.SendAuthorizationMessage(client);
                                Logs.Add($"Подключился {client.Client.RemoteEndPoint} ошибка при авторизации");
                            }
                            else {
                                await SendMessageClient.SendСonnectionMessage(client, employee);
                                Logs.Add($"Подключился {client.Client.RemoteEndPoint} прошел авторизацию");
                            }
                        }

                    }
                    else if (message == Message.SurveyList) {
                        buffer = await client.ReadFromStream(4);
                        int employeeId = BitConverter.ToInt32(buffer, 0);
                        Survey[] surveys;
                        using (SurveysContext context = new()) {
                            surveys = context.Surveys.Where(survey => !survey.Employees.Any(employee => employee.Id == employeeId))
                                .Include("Questions")
                                .ToArray();
                            surveys.ForEach(survey => survey.Questions.ForEach(
                                question => {
                                    context.Entry(question).Reference("Type").Load();
                                    context.Entry(question).Collection("FreeAnswers").Load();
                                    context.Entry(question).Collection("MultipleAnswers").Load();
                                    context.Entry(question).Collection("SingleAnswers").Load();
                                })
                            );
                        }
                        await SendMessageClient.SendSurveyListMessage(client, surveys);
                    }
                    else if (message == Message.AllSurveyAndQuestionTypes) {
                        Survey[] surveys;
                        QuestionType[] questionTypes;
                        using (SurveysContext context = new()) {
                            context.QuestionTypes.Load();
                            context.Surveys.Load();
                            context.Questions.Load();
                            context.Questions.Include("FreeAnswers")
                                .Include("MultipleAnswers")
                                .Include("SingleAnswers");
                            surveys = context.Surveys.ToArray();
                            questionTypes = context.QuestionTypes.ToArray();
                        }
                        await SendMessageClient.SendAllSurveyAndQuestionTypeMessage(client, surveys, questionTypes);
                    }

                    else if (message == Message.EmployeeAnswers) {
                        buffer = await client.ReadFromStream(4);
                        buffer = await client.ReadFromStream(BitConverter.ToInt32(buffer, 0));

                        EmployeeSurveyAnswer employeeAnswer = JsonSerializer.Deserialize<EmployeeSurveyAnswer>(
                            Encoding.UTF8.GetString(buffer),
                            new() { IncludeFields = true }
                            );

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

                        await SendMessageClient.SendDataSaveSuccessMessage(client, "Данные успешно сохранены!");
                    }

                    else if (message == Message.EmployeeList) {

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
                    }
                    else if (message == Message.AllAnswerEmployee) {
                        buffer = await client.ReadFromStream(4);
                        int employeeId = BitConverter.ToInt32(buffer, 0);
                        Survey[] surveys;
                        using (SurveysContext context = new()) {
                            surveys = context.Surveys.Where(survey => survey.Employees.Any(employee => employee.Id == employeeId))
                                .Include("Questions")
                                .ToArray();
                            surveys.ForEach(survey => survey.Questions.ForEach(
                                question => {
                                    context.Entry(question).Reference("Type").Load();
                                    context.Entry(question).Collection("FreeAnswers").Load();
                                    context.Entry(question).Collection("MultipleAnswers").Load();
                                    context.Entry(question).Collection("SingleAnswers").Load();
                                })
                            );
                        }
                    }


                }
            }
            catch (Exception ex) {
                MessageBox.Show(ex.Message);
                lock (_clients) {
                    Logs.Add($"Отключился {client.Client.RemoteEndPoint} {DateTime.Now}");
                    if (client.Client.Connected)
                        client.Client.Shutdown(SocketShutdown.Both);
                    client.Client.Close();
                    _clients.Remove(client);
                }
            }
        }
    }
}
