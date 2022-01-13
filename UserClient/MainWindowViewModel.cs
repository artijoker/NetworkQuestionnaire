using Library;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;

namespace UserClient {
    class MainWindowViewModel : INotifyPropertyChanged {
        
        private readonly TcpClient _server;
        private bool _isEnabledInterface;
        private bool _isHide;
        private Visibility _visibilityProcess;
        private Survey _selectedSurvay;
        private string _text;


        public event PropertyChangedEventHandler? PropertyChanged;

        public Employee Employee { get; }
        public ObservableCollection<Survey> Surveys { get; } = new ObservableCollection<Survey>();

        public bool IsEnabledInterface {
            get => _isEnabledInterface;
            set {
                _isEnabledInterface = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsEnabledInterface)));
            }
        }

        public bool IsHide {
            get => _isHide;
            set {
                _isHide = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsHide)));
            }
        }

        public Visibility VisibilityProcess {
            get => _visibilityProcess;
            set {
                _visibilityProcess = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(VisibilityProcess)));
            }
        }

        public Survey SelectedSurvay {
            get => _selectedSurvay;
            set {
                _selectedSurvay = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SelectedSurvay)));
            }
        }

        public string Text {
            get => _text;
            set {
                _text = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Text)));
            }
        }

        public DelegateCommand LoadingSurveyListCommand { get; }
        public DelegateCommand TakeSurveyCommand { get; }

        public MainWindowViewModel(TcpClient server, Employee employee) {
            _server = server;
            Employee = employee;
            IsEnabledInterface = true;
            VisibilityProcess = Visibility.Hidden;
            LoadingSurveyListCommand = new(LoadingSurveyList);
            TakeSurveyCommand = new(TakeSurvey);
            LoadingSurveyList();
            ListenToServer();
        }
        private async void ListenToServer() {
            try {
                while (true) {
                    byte[] buffer = await _server.ReadFromStream(1);
                    byte message = buffer[0];

                    if (message == Message.SurveyList) {
                        buffer = await _server.ReadFromStream(4);
                        buffer = await _server.ReadFromStream(BitConverter.ToInt32(buffer, 0));

                        var surveys = JsonSerializer.Deserialize<SurveyDTO[]>(Encoding.UTF8.GetString(buffer))
                            .Select(surveyDTO => Survey.FromDTO(surveyDTO));

                        Surveys.Clear();
                        foreach (var survey in surveys) 
                            Surveys.Add(survey);
                        IsEnabledInterface = true;
                        VisibilityProcess = Visibility.Hidden;
                    }
                    //else if (message == Message.SelectedSurvey) {
                    //    buffer = await _server.ReadFromStream(4);
                    //    buffer = await _server.ReadFromStream(BitConverter.ToInt32(buffer, 0));

                    //    Survey survey = Survey.FromDTO(JsonSerializer.Deserialize<SurveyDTO>(Encoding.UTF8.GetString(buffer)));

                    //    IsEnabledInterface = true;
                    //    VisibilityProcess = Visibility.Hidden;
                    //    IsHide = true;

                    //    SurveyWindow dialog = new SurveyWindow(survey);
                    //    if (dialog.ShowDialog() == true) {
                    //        EmployeeSurveyAnswerDTO employeeSurveyAnswer = dialog.ViewModel.EmployeeSurveyAnswer;
                    //        employeeSurveyAnswer.Employee = Employee.ToDTO();
                    //        employeeSurveyAnswer.Survey = SelectedSurvay.ToDTO();

                    //        string jsonString = JsonSerializer.Serialize(employeeSurveyAnswer);
                    //        File.WriteAllText("Answers.json", jsonString);

                    //        IsHide = false;
                    //        IsEnabledInterface = false;
                    //        VisibilityProcess = Visibility.Visible;
                    //        Text = "Идет процесс сохранения данных. Пожалуйста подождите.";

                    //        await SendMessageServer.SendEmployeeAnswerMessage(_server, employeeSurveyAnswer);
                    //    }
                    //    IsHide = false;
                        
                        
                    //}
                    else if (message == Message.DataSaveSuccess) {
                        buffer = await _server.ReadFromStream(4);
                        buffer = await _server.ReadFromStream(BitConverter.ToInt32(buffer, 0));
                        Surveys.Remove(SelectedSurvay);
                        MessageBox.Show(Encoding.UTF8.GetString(buffer));
                        IsEnabledInterface = true;
                        VisibilityProcess = Visibility.Hidden;
                    }

                }
            }
            catch (Exception ex) {
                MessageBox.Show(ex.Message);
                BreakConnection();
                IsEnabledInterface = true;
                VisibilityProcess = Visibility.Hidden;
                return;
            }
        }

        private async void LoadingSurveyList() {
            IsEnabledInterface = false;
            VisibilityProcess = Visibility.Visible;
            Text = "Идет процесс обновления списка опросов. Пожалуйста подождите.";
            await SendMessageServer.SendSurveyListMessage(_server, Employee);
        }

        private async void TakeSurvey() {
            if (SelectedSurvay is null || Surveys.Count == 0) 
                return;
            
            SurveyWindow dialog = new(SelectedSurvay);
            if (dialog.ShowDialog() == true) {
                EmployeeSurveyAnswerDTO employeeSurveyAnswer = dialog.ViewModel.EmployeeSurveyAnswer;
                employeeSurveyAnswer.Employee = Employee.ToDTO();
                employeeSurveyAnswer.Survey = SelectedSurvay.ToDTO();

                string jsonString = JsonSerializer.Serialize(employeeSurveyAnswer);
                File.WriteAllText("Answers.json", jsonString);

                IsHide = false;
                IsEnabledInterface = false;
                VisibilityProcess = Visibility.Visible;
                Text = "Идет процесс сохранения данных. Пожалуйста подождите.";

                await SendMessageServer.SendEmployeeAnswerMessage(_server, employeeSurveyAnswer);
            }
            IsHide = false;
        }


        private void BreakConnection() {
            if (_server.Client.Connected)
                _server.Client.Shutdown(SocketShutdown.Both);
            _server.Client.Close();
        }
    }
}
