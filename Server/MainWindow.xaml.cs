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
                        //buffer = await client.ReadFromStream(4);
                        //int employeeId = BitConverter.ToInt32(buffer, 0);
                        //Survey[] surveys;
                        //using (SurveysContext context = new()) {
                        //    Employee employee = context.Employees.Where(employee => employee.Id == employeeId).Single();
                        //    context.EmployeesFreeAnswers.Where(e => e.EmployeeId == employeeId).Reference("Type")
                        //    surveys = context.Surveys.Where(survey => survey.Employees.Any(employee => employee.Id == employeeId))
                        //        .Include("Questions")
                        //        .ToArray();
                        //    surveys.ForEach(survey => survey.Questions.ForEach(
                        //        question => {
                        //            context.Entry(question).Reference("Type").Load();
                        //            context.Entry(question).Collection("FreeAnswers").Load();
                        //            context.Entry(question).Collection("MultipleAnswers").Load();
                        //            context.Entry(question).Collection("SingleAnswers").Load();

                        //            question.FreeAnswers.ForEach(answer => answer.EmployeeFreeAnswers))
                        //        })
                        //    );
                        //}
                    }
                    else if (message == Message.AddNewSurvey) {
                        buffer = await client.ReadFromStream(4);
                        buffer = await client.ReadFromStream(BitConverter.ToInt32(buffer, 0));

                        Survey survey = Survey.FromDTO(JsonSerializer.Deserialize<SurveyDTO>(Encoding.UTF8.GetString(buffer)));

                        using (SurveysContext context = new()) {

                            context.Surveys.Add(survey);
                            context.SaveChanges();
                        }
                        await SendMessageClient.SendDataSaveSuccessMessage(client, "Новый опрос добавлен в базу данных!");
                    }
                    else if (message == Message.EditSurvey) {
                        buffer = await client.ReadFromStream(4);
                        buffer = await client.ReadFromStream(BitConverter.ToInt32(buffer, 0));

                        Survey survey = Survey.FromDTO(JsonSerializer.Deserialize<SurveyDTO>(Encoding.UTF8.GetString(buffer)));
                        foreach (var question in survey.Questions)
                            question.Type = null;
                        UpdateSurvey(survey);
                        //Question[]? questions = context.Questions.Where(question => question.SurveyId == modifiedSurvey.Id).ToArray();
                        //context.RemoveRange(questions.Where(question => !modifiedSurvey.Questions.Any(q => q.Id == question.Id)).ToArray());
                        //context.Questions.AddRange(modifiedSurvey.Questions.Where(question => !context.Questions.Any(q => q.Id == question.Id)).ToArray());

                        await SendMessageClient.SendDataSaveSuccessMessage(client, "Измененые опрос зафиксирован в базе данных!");
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
        private void UpdateSurvey(Survey survey) {
            using (SurveysContext context = new()) {
                //Survey surveyFromDB = context.Surveys
                //    .Where(s => s.Id == survey.Id)
                //    .Include(s => s.Questions)
                //    .Single();
                //surveyFromDB.Questions.ForEach(
                //    question => {
                //        context.Entry(question).Collection("FreeAnswers").Load();
                //        context.Entry(question).Collection("MultipleAnswers").Load();
                //        context.Entry(question).Collection("SingleAnswers").Load();
                //    });

                //Update
                //context.Entry(surveyFromDB).CurrentValues.SetValues(survey);

                Question[]? questions = context.Questions.Where(question => question.SurveyId == survey.Id).ToArray();
                context.Questions.RemoveRange(questions.Where(question => !survey.Questions.Any(q => q.Id == question.Id)).ToArray());
                context.Questions.AddRange(survey.Questions.Where(question => !context.Questions.Any(q => q.Id == question.Id)).ToArray());

                questions = survey.Questions.Where(question => context.Questions.Any(q => q.Id == question.Id && question.Id != 0)).ToArray();

                foreach (Question question in questions)
                    UpdateQuestion(context, question);

                Survey surveyFromDB =  context.Surveys.Where(s => s.Id == survey.Id).Single();
                surveyFromDB.Name = survey.Name;
                Employee[]? employees = context.Employees.Where(e => e.Surveys.Any(s => s.Id == survey.Id)).ToArray();
                employees.ForEach(e => e.Surveys.Remove(surveyFromDB));
                surveyFromDB.Employees.Clear();
                //surveyFromDB.Employees.Clear();

                //context.Entry(survey).State = EntityState.Modified;

                ////Delete children
                //foreach (Question questionFromDB in surveyFromDB.Questions) {
                //    if (!survey.Questions.Any(q => q.Id == questionFromDB.Id))
                //        context.Questions.Remove(questionFromDB);
                //}

                ////Insert children
                //foreach (Question question in survey.Questions) {
                //    if (!surveyFromDB.Questions.Any(q => q.Id == question.Id))
                //        context.Questions.Add(question);
                //}

                //foreach (Question question in survey.Questions) {
                //    Question? questionFromDB = surveyFromDB.Questions
                //        .Where(q => q.Id == question.Id && question.Id != 0)
                //        .SingleOrDefault();

                //    if (questionFromDB is not null)
                //        UpdateQuestion(context, question);
                //}

                context.SaveChanges();
            }
        }

        private void UpdateQuestion(SurveysContext context, Question question) {
            //Question newQuestion = question;
            //Question oldQuestion = context.Questions
            //        .Where(q => q.Id == question.Id)
            //        .Include(q => q.FreeAnswers).Include(q => q.MultipleAnswers).Include(q => q.SingleAnswers)
            //        .Single();
            //context.Entry(oldQuestion).CurrentValues.SetValues(newQuestion);

            SingleAnswer[]? singleAnswers = context.SingleAnswers.Where(answer => answer.QuestionId == question.Id).ToArray();
            context.SingleAnswers.RemoveRange(singleAnswers.Where(answer => !question.SingleAnswers.Any(a => a.Id == answer.Id)).ToArray());
            context.SingleAnswers.AddRange(question.SingleAnswers.Where(answer => !context.SingleAnswers.Any(a => a.Id == answer.Id)).ToArray());
            singleAnswers = question.SingleAnswers.Where(answer => context.SingleAnswers.Any(a => a.Id == answer.Id && answer.Id != 0)).ToArray();

            foreach (SingleAnswer answer in singleAnswers) {
                answer.Text = question.SingleAnswers.Where(a => a.Id == answer.Id).Single().Text;
                Employee[]? employees = context.Employees.Where(e => e.SingleAnswers.Any(s => s.Id == answer.Id)).ToArray();
                employees.ForEach(e => e.SingleAnswers.Remove(answer));
                answer.Employees.Clear();
            }


            MultipleAnswer[]? multipleAnswers = context.MultipleAnswers.Where(answer => answer.QuestionId == question.Id).ToArray();
            context.MultipleAnswers.RemoveRange(multipleAnswers.Where(answer => !question.MultipleAnswers.Any(a => a.Id == answer.Id)).ToArray());
            context.MultipleAnswers.AddRange(question.MultipleAnswers.Where(answer => !context.MultipleAnswers.Any(a => a.Id == answer.Id)).ToArray());
            multipleAnswers = question.MultipleAnswers.Where(answer => context.MultipleAnswers.Any(a => a.Id == answer.Id && answer.Id != 0)).ToArray();

            foreach (MultipleAnswer answer in multipleAnswers) {
                answer.Text = question.MultipleAnswers.Where(a => a.Id == answer.Id).Single().Text;
                Employee[]? employees = context.Employees.Where(e => e.MultipleAnswers.Any(s => s.Id == answer.Id)).ToArray();
                employees.ForEach(e => e.MultipleAnswers.Remove(answer));
                answer.Employees.Clear();
            }


            FreeAnswer? freeAnswer = context.FreeAnswers.Where(answer => answer.QuestionId == question.Id).SingleOrDefault();
            if (freeAnswer is not null) {
                freeAnswer.EmployeeFreeAnswers.Clear();
                Employee[]? employees = context.Employees.Where(e => e.EmployeeFreeAnswers.Any(e => e.FreeAnswerId == freeAnswer.Id)).ToArray();
                //employees.ForEach(e => e.EmployeeFreeAnswers.Remove();
                //answer.Employees.Clear();
            }
                context.FreeAnswers.Remove(freeAnswer);

            if (freeAnswer is null && question.FreeAnswers.Count != 0)
                context.FreeAnswers.Add(question.FreeAnswers.Single());

            Question questionFromDB = context.Questions.Where(q => q.Id == question.Id).Single();
            questionFromDB.IsRequired = question.IsRequired;
            questionFromDB.Text = question.Text;
            questionFromDB.QuestionTypeId = question.QuestionTypeId;

            //context.SingleAnswers.RemoveRange(singleAnswers.Where(answer => !question.SingleAnswers.Any(a => a.Id == answer.Id)).ToArray());
            //context.SingleAnswers.AddRange(question.SingleAnswers.Where(answer => !context.SingleAnswers.Any(a => a.Id == answer.Id)).ToArray());
            //singleAnswers = question.SingleAnswers.Where(answer => context.SingleAnswers.Any(a => a.Id == answer.Id && answer.Id != 0)).ToArray();

            //foreach (SingleAnswer answer in singleAnswers)
            //    context.Entry(answer).State = EntityState.Modified;




            //Update question
            //context.Entry(oldQuestion).CurrentValues.SetValues(newQuestion);



            //Update SingleAnswers
            //Delete children
            //if (newQuestion.SingleAnswers.Count == 0)
            //    context.SingleAnswers.RemoveRange(oldQuestion.SingleAnswers);
            //else {
            //    foreach (SingleAnswer answerFromDB in oldQuestion.SingleAnswers) {
            //        if (!newQuestion.SingleAnswers.Any(a => a.Id == answerFromDB.Id))
            //            context.SingleAnswers.Remove(answerFromDB);
            //    }
            //    //Insert children
            //    foreach (SingleAnswer answer in newQuestion.SingleAnswers) {
            //        if (!oldQuestion.SingleAnswers.Any(a => a.Id == answer.Id))
            //            context.SingleAnswers.Add(answer);
            //    }
            //    //Update children
            //    foreach (SingleAnswer answer in newQuestion.SingleAnswers) {
            //        SingleAnswer? singleAnswer = oldQuestion.SingleAnswers
            //            .Where(a => a.Id == answer.Id && answer.Id != 0)
            //            .SingleOrDefault();

            //        if (singleAnswer is not null)
            //            context.Entry(singleAnswer).CurrentValues.SetValues(answer);
            //    }
            //}



            ////Update MultipleAnswers
            ////Delete children
            //if (newQuestion.MultipleAnswers.Count == 0) {
            //    context.MultipleAnswers.RemoveRange(oldQuestion.MultipleAnswers);
            //}
            //else {
            //    foreach (MultipleAnswer answerFromDB in oldQuestion.MultipleAnswers) {
            //        if (!newQuestion.MultipleAnswers.Any(a => a.Id == answerFromDB.Id))
            //            context.MultipleAnswers.Remove(answerFromDB);
            //    }
            //    //Insert children
            //    foreach (MultipleAnswer answer in newQuestion.MultipleAnswers) {
            //        if (!oldQuestion.MultipleAnswers.Any(a => a.Id == answer.Id))
            //            context.MultipleAnswers.Add(answer);
            //    }
            //    //Update children
            //    foreach (MultipleAnswer answer in newQuestion.MultipleAnswers) {
            //        MultipleAnswer? multipleAnswer = oldQuestion.MultipleAnswers
            //            .Where(a => a.Id == answer.Id && answer.Id != 0)
            //            .SingleOrDefault();

            //        if (multipleAnswer is not null)
            //            context.Entry(multipleAnswer).CurrentValues.SetValues(answer);
            //    }
            //}


            ////Update FreeAnswers
            ////Delete children
            //foreach (FreeAnswer answerFromDB in oldQuestion.FreeAnswers) {
            //    if (newQuestion.FreeAnswers.Count == 0)
            //        context.FreeAnswers.Remove(answerFromDB);
            //}
            ////Insert children
            //foreach (FreeAnswer answer in newQuestion.FreeAnswers) {
            //    if (oldQuestion.FreeAnswers.Count == 0)
            //        context.FreeAnswers.Add(answer);
            //}
        }
    }
}
