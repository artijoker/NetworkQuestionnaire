using Library;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace AdminClient {
    class MainWindowViewModel : INotifyPropertyChanged {
        private bool _isHide;
        private bool _isClose;
        private readonly TcpClient _server;
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
            _server = new TcpClient(_ipAddress, _port);
            ShowEmployeesCommand = new DelegateCommand(ShowEmployees);
            ShowSurveysCommand = new DelegateCommand(ShowSurveys);
        }


        private void ShowEmployees() {
            EmployeesWindow dialog = new(_server);
            IsHide = true;
            dialog.ShowDialog();
            IsClose = true;
            return;
        }

        private void ShowSurveys() {
            SurveysWindow dialog = new(_server);
            IsHide = true;
            dialog.ShowDialog();
            IsClose = true;
            return;
        }

    }
}
