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

namespace AdminClient {
    class SurveysWindowViewModel : INotifyPropertyChanged {
        private readonly TcpClient _server;
        private QuestionType[] _questionTypes;
        private bool _isEnabledInterface;
        private bool _isHide;
        private Visibility _visibilityProcess;

        private Survey _selectedSurvey;
        private string _text;

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

        public Survey SelectedSurvey {
            get => _selectedSurvey;
            set {
                _selectedSurvey = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SelectedSurvey)));
            }
        }

        public string Text {
            get => _text;
            set {
                _text = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Text)));
            }
        }
        public DelegateCommand AddSurveyCommand { get; }
        public DelegateCommand EditSurveyCommand { get; }
        public DelegateCommand RemoveSurveyCommand { get; }
        public DelegateCommand ReadyCommand { get; }

        public SurveysWindowViewModel(TcpClient server) {
            _server = server;
            IsEnabledInterface = true;
            VisibilityProcess = Visibility.Hidden;
            AddSurveyCommand = new(AddSurvey);
            EditSurveyCommand = new(EditSurvey);
            RemoveSurveyCommand = new(RemoveSurvey);
            Text = "Идет процесс создания списка опросов. Пожалуйста подождите.";
            LoadingSurveyList();
            ListenToServer();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private async void ListenToServer() {
            try {
                while (true) {
                    byte[] buffer = await _server.ReadFromStream(1);
                    byte message = buffer[0];

                    if (message == Message.AllSurveyAndQuestionTypes) {
                        buffer = await _server.ReadFromStream(4);
                        buffer = await _server.ReadFromStream(BitConverter.ToInt32(buffer, 0));

                        //string json = Encoding.UTF8.GetString(buffer);
                        //File.WriteAllText("NewTest.json", json);
                        IEnumerable<Survey> surveys = JsonSerializer.Deserialize<SurveyDTO[]>(Encoding.UTF8.GetString(buffer))
                             .Select(surveyDTO => Survey.FromDTO(surveyDTO));

                        Surveys.Clear();
                        foreach (var survey in surveys)
                            Surveys.Add(survey);

                        buffer = await _server.ReadFromStream(4);
                        buffer = await _server.ReadFromStream(BitConverter.ToInt32(buffer, 0));

                        _questionTypes = JsonSerializer.Deserialize<QuestionTypeDTO[]>(Encoding.UTF8.GetString(buffer))
                             .Select(questionTypesDTO => QuestionType.FromDTO(questionTypesDTO)).ToArray();

                        VisibilityProcess = Visibility.Hidden;
                        IsEnabledInterface = true;
                    }
                    else if (message == Message.DataSaveSuccess) {
                        buffer = await _server.ReadFromStream(4);
                        buffer = await _server.ReadFromStream(BitConverter.ToInt32(buffer, 0));
                        VisibilityProcess = Visibility.Hidden;
                        MessageBox.Show(
                           Encoding.UTF8.GetString(buffer),
                           "",
                           MessageBoxButton.OK,
                           MessageBoxImage.Information
                           );
                        Text = "Идет процесс обновления списка опросов. Пожалуйста подождите.";
                        LoadingSurveyList();
                    }

                }
            }
            catch (Exception ex) {
                MessageBox.Show(ex.Message);
                BreakConnection();
                Application.Current.MainWindow.Close();
                return;
            }
        }

        private void BreakConnection() {
            if (_server.Client.Connected)
                _server.Client.Shutdown(SocketShutdown.Both);
            _server.Client.Close();
        }

        private async void LoadingSurveyList() {
            IsEnabledInterface = false;
            VisibilityProcess = Visibility.Visible;
            await SendMessageServer.SendSurveyListMessage(_server);
        }

        private async void AddSurvey() {
            AddEditSurveyWindow dialog = new(_questionTypes);
            if (dialog.ShowDialog() == true) {
                IsEnabledInterface = false;
                VisibilityProcess = Visibility.Visible;
                Survey survey = dialog.ViewModel.Survey;
                //Survey survey = Test();
                //string jsonString = JsonSerializer.Serialize(survey);
                //File.WriteAllText("SurveysTest.json", jsonString);
                //string json = File.ReadAllText("SurveysTest.json");
                Text = "Идет процесс добавления нового опроса. Пожалуйста подождите.";
                await SendMessageServer.SendAddNewSurveyMessage(_server, survey);
            }
        }

        private async void EditSurvey() {
            if (SelectedSurvey is null)
                return;
            AddEditSurveyWindow dialog = new(_questionTypes, SelectedSurvey);
            if (dialog.ShowDialog() == true) {
                IsEnabledInterface = false;
                VisibilityProcess = Visibility.Visible;
                Survey survey = dialog.ViewModel.Survey;
                Text = "Идет процесс изменения выбраного опроса. Пожалуйста подождите.";
                await SendMessageServer.SendEditSurveyMessage(_server, survey);
            }
        }

        private async void RemoveSurvey() {
            if (SelectedSurvey is null)
                return;
            IsEnabledInterface = false;
            VisibilityProcess = Visibility.Visible;
            Text = "Идет процесс удаления выбраного опроса. Пожалуйста подождите.";
            await SendMessageServer.SendRemoveSurveyMessage(_server, SelectedSurvey);
        }


        private Survey Test() {
            Survey survey = new Survey() {
                Name = "TEST"
            };
            survey.Questions.Add(
                new Question() {
                    QuestionTypeId = 1,
                    IsRequired = true,
                    Text = "WHAT1?",
                    SingleAnswers = new SingleAnswer[] {
                        new SingleAnswer() {
                            Text = "01"
                        },
                        new SingleAnswer() {
                            Text = "02"
                        }
                    }
                }
            );
            survey.Questions.Add(
                new Question() {
                    QuestionTypeId = 1,
                    IsRequired = true,
                    Text = "WHAT2?",
                    SingleAnswers = new SingleAnswer[] {
                        new SingleAnswer() {
                            Text = "03"
                        },
                        new SingleAnswer() {
                            Text = "04"
                        }
                    }

                }
            );
            survey.Questions.Add(
                new Question() {
                    QuestionTypeId = 1,
                    IsRequired = true,
                    Text = "WHAT3?",
                    SingleAnswers = new SingleAnswer[] {
                        new SingleAnswer() {
                            Text = "05"
                        },
                        new SingleAnswer() {
                            Text = "06"
                        }
                    }
                }
            );
            return survey;

        }
    }

}
