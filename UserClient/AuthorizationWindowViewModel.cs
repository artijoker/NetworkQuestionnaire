using Library;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;

namespace UserClient {
    public class AuthorizationWindowViewModel : INotifyPropertyChanged {
        private string _login;
        private string _password;
        private bool _isHide;
        private bool _isClose;
        private bool _isEnabledInterface;
        private Visibility _visibilityAuthorizationProcess;
        private TcpClient _server;
        private readonly string _ipAddress;
        private readonly int _port;

        public event PropertyChangedEventHandler PropertyChanged;
        
        public string Login {
            get => _login;
            set {
                _login = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Login)));
            }
        }

        public string Password {
            get => _password;
            set {
                _password = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Password)));
            }
        }

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

        public Visibility VisibilityAuthorizationProcess {
            get => _visibilityAuthorizationProcess;
            set {
                _visibilityAuthorizationProcess = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(VisibilityAuthorizationProcess)));
            }
        }

        public DelegateCommand EnterCommand { get; }

        public AuthorizationWindowViewModel() {
            EnterCommand = new DelegateCommand(Enter);
            IConfiguration config = Host.CreateDefaultBuilder().Build().Services.GetRequiredService<IConfiguration>();
            _ipAddress = config.GetValue<string>("IpAddress");
            _port = config.GetValue<int>("Port");
        }
        public void WindowLoaded() {
            try {
                _server = new(_ipAddress, _port);
                IsEnabledInterface = true;
                VisibilityAuthorizationProcess = Visibility.Hidden;
                Authorization();
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

        private async void Enter() {
            if (string.IsNullOrEmpty(_login)) {
                MessageBox.Show(
                    "Введите логин!",
                    "Логин",
                    MessageBoxButton.OK,
                    MessageBoxImage.Exclamation
                    );
                return;
            }
            if (string.IsNullOrEmpty(_password)) {
                MessageBox.Show(
                    "Введите пароль!",
                    "Пароль",
                    MessageBoxButton.OK,
                    MessageBoxImage.Exclamation
                    );
                return;
            }
            IsEnabledInterface = false;
            VisibilityAuthorizationProcess = Visibility.Visible;
            await SendMessageServer.SendAuthorizationMessage(_server, _login, _password);
            
        }

        private async void Authorization() {
            try {
                while (true) {

                    byte[] buffer = await _server.ReadFromStream(1);
                    byte message = buffer[0];

                    if (message == Message.AuthorizationFailed) {
                        buffer = await _server.ReadFromStream(4);
                        buffer = await _server.ReadFromStream(BitConverter.ToInt32(buffer, 0));
                        MessageBox.Show(
                           Encoding.UTF8.GetString(buffer),
                           "Ошибка",
                           MessageBoxButton.OK,
                           MessageBoxImage.Error
                           );
                        IsEnabledInterface = true;
                        VisibilityAuthorizationProcess = Visibility.Hidden;
                    }
                    else if (message == Message.AuthorizationSuccessful) {
                        buffer = await _server.ReadFromStream(4);
                        buffer = await _server.ReadFromStream(BitConverter.ToInt32(buffer, 0));
                        Employee employee = Employee.FromDTO(JsonSerializer.Deserialize<EmployeeDTO>(Encoding.UTF8.GetString(buffer)));
                        IsHide = true;
                        MainWindow dialog = new(_server, employee);
                        dialog.ShowDialog();
                        IsClose = true;
                        return;
                    }
                }
            }
            catch (Exception ex) {
                MessageBox.Show(ex.Message);
                if (_server.Client.Connected)
                    _server.Client.Shutdown(SocketShutdown.Both);
                _server.Client.Close();
                IsClose = true;
                return;
            }
        }


    }
}
