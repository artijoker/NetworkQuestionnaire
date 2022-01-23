using Library;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace AdminClient {
    class MainWindowViewModel : INotifyPropertyChanged {
        private bool _isHide;
        private bool _isClose;
        private TcpClient _server;
        private string _ipAddress = "127.0.0.1";
        private int _port = 56537;
        private bool _isEnabledInterface;
        private Visibility _visibilityConnectionProcess;


        public event PropertyChangedEventHandler PropertyChanged;

        public bool IsHide {
            get => _isHide;
            set {
                _isHide = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsHide)));
            }
        }
        public bool IsClose {
            get => _isClose;
            set {
                _isClose = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsClose)));
            }
        }

        public bool IsEnabledInterface {
            get => _isEnabledInterface;
            set {
                _isEnabledInterface = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsEnabledInterface)));
            }
        }

        public Visibility VisibilityConnectionProcess {
            get => _visibilityConnectionProcess;
            set {
                _visibilityConnectionProcess = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(VisibilityConnectionProcess)));
            }
        }

        public DelegateCommand ShowEmployeesCommand { get; }
        public DelegateCommand ShowSurveysCommand { get; }

        public MainWindowViewModel() {
            
            ShowEmployeesCommand = new DelegateCommand(ShowEmployees);
            ShowSurveysCommand = new DelegateCommand(ShowSurveys);
            VisibilityConnectionProcess = Visibility.Hidden;
            IsEnabledInterface = true;
        }
        public async void WindowLoaded() {
            try {
                _server = new(_ipAddress, _port);
                VisibilityConnectionProcess = Visibility.Visible;
                IsEnabledInterface = false;
                await SendMessageServer.SendAdminConnectMessage(_server);
                ListenToServer();
            }
            catch (SocketException ex) {
                MessageBox.Show(
                    ex.Message, 
                    "Ошибка соединения", 
                    MessageBoxButton.OK, 
                    MessageBoxImage.Error
                    );
                IsClose = true;
            }
        }
        private async void ListenToServer() {
            try {
                while (true) {
                    byte[] buffer = await _server.ReadFromStream(1);
                    byte message = buffer[0];

                    if (message == Message.AdminConnectSuccessful) {
                        MessageBox.Show(
                           "Подключение прошло успешно!",
                           "",
                           MessageBoxButton.OK,
                           MessageBoxImage.Information
                           );
                        VisibilityConnectionProcess = Visibility.Hidden;
                        IsEnabledInterface = true;
                        return;
                    }
                    else if (message == Message.AdminConnectFailed) {
                        buffer = await _server.ReadFromStream(4);
                        buffer = await _server.ReadFromStream(BitConverter.ToInt32(buffer, 0));
                        MessageBox.Show(
                           Encoding.UTF8.GetString(buffer),
                           "Ошибка!",
                           MessageBoxButton.OK,
                           MessageBoxImage.Error
                           );
                        VisibilityConnectionProcess = Visibility.Hidden;
                    }
                }
            }
            catch (Exception) {
                if (_server.Client.Connected)
                    _server.Client.Shutdown(SocketShutdown.Both);
                _server.Client.Close();
                return;
            }
        }

        private void ShowEmployees() {
            EmployeesWindow dialog = new(_server);
            IsHide = true;
            dialog.ShowDialog();
            IsClose = true;
        }

        private void ShowSurveys() {
            SurveysWindow dialog = new(_server);
            IsHide = true;
            dialog.ShowDialog();
            IsClose = true;
        }

    }
}
