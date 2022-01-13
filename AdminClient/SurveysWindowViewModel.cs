using Library;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace AdminClient {
    class SurveysWindowViewModel : INotifyPropertyChanged {
        private readonly TcpClient _server;
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

        public SurveysWindowViewModel(TcpClient server) {

        }

        public event PropertyChangedEventHandler PropertyChanged;

        private async void ListenToServer() {
            try {
                while (true) {
                    byte[] buffer = await _server.ReadFromStream(1);
                    byte message = buffer[0];


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
    }
}
