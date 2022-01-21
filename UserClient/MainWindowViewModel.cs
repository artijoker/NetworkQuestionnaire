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

                        ICollection<Survey> surveys = JsonSerializer.Deserialize<SurveyDTO[]>(Encoding.UTF8.GetString(buffer))
                            .Select(surveyDTO => Survey.FromDTO(surveyDTO)).ToArray();
                        VisibilityProcess = Visibility.Hidden;
                        if (surveys.Count == 0) {
                            MessageBox.Show(
                           "Для вас нет новых опросов!",
                           "Загрузка завершена",
                           MessageBoxButton.OK,
                           MessageBoxImage.Information
                           );
                        }
                        else {
                            Surveys.Clear();
                            foreach (var survey in surveys)
                                Surveys.Add(survey);
                            MessageBox.Show(
                           "Готово!",
                           "Загрузка завершена",
                           MessageBoxButton.OK,
                           MessageBoxImage.Information
                           );
                        }
                        IsEnabledInterface = true;
                    }
 
                    else if (message == Message.DataSaveSuccess) {
                        buffer = await _server.ReadFromStream(4);
                        buffer = await _server.ReadFromStream(BitConverter.ToInt32(buffer, 0));
                        Surveys.Remove(SelectedSurvay);

                        VisibilityProcess = Visibility.Hidden;
                        MessageBox.Show(
                           Encoding.UTF8.GetString(buffer),
                           "",
                           MessageBoxButton.OK,
                           MessageBoxImage.Information
                           );
                        IsEnabledInterface = true;
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
            Text = "Идет процесс загрузки списка опросов. Пожалуйста подождите.";
            await SendMessageServer.SendSurveyListMessage(_server, Employee);
        }

        private async void TakeSurvey() {
            if (SelectedSurvay is null || Surveys.Count == 0) 
                return;
            
            SurveyWindow dialog = new(SelectedSurvay);
            // Survey survey = new() { Id = SelectedSurvay.Id, Name = SelectedSurvay.Name };
            //employeeSurveyAnswer.Survey = survey.ToDTO();
            if (dialog.ShowDialog() == true) {
                EmployeeSurveyAnswer employeeSurveyAnswer = dialog.ViewModel.EmployeeSurveyAnswer;
                employeeSurveyAnswer.EmployeeId = Employee.Id;
                employeeSurveyAnswer.SurveyId = SelectedSurvay.Id;
                //JsonSerializerOptions options = new() { IncludeFields = true };
                //string jsonString = JsonSerializer.Serialize(employeeSurveyAnswer, options);
                //File.WriteAllText("Answers.json", jsonString);

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
