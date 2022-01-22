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

        public DelegateCommand ShowEmployeesCommand { get; }
        public DelegateCommand ShowSurveysCommand { get; }

        public MainWindowViewModel() {
            
            ShowEmployeesCommand = new DelegateCommand(ShowEmployees);
            ShowSurveysCommand = new DelegateCommand(ShowSurveys);
        }
        public void WindowLoaded() {
            try {
                _server = new(_ipAddress, _port);
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
