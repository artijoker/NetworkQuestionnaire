using Library;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;

namespace AdminClient {
    class EmployeesWindowViewModel : INotifyPropertyChanged {
        private readonly TcpClient _server;
        private bool _isEnabledInterface;
        private bool _isHide;
        private Visibility _visibilityProcess;
        private Employee _selectedEmployee;
        private string _text;

        public event PropertyChangedEventHandler PropertyChanged;

        public ObservableCollection<Employee> Employees { get; } = new ObservableCollection<Employee>();

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

        public Employee SelectedEmployee {
            get => _selectedEmployee;
            set {
                _selectedEmployee = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SelectedEmployee)));
            }
        }

        public string Text {
            get => _text;
            set {
                _text = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Text)));
            }
        }
        public DelegateCommand AddEmployeeCommand { get; }
        public DelegateCommand EditEmployeeCommand { get; }
        public DelegateCommand RemoveEmployeeCommand { get; }

        public EmployeesWindowViewModel(TcpClient server) {
            _server = server;
            IsEnabledInterface = true;
            VisibilityProcess = Visibility.Hidden;
            AddEmployeeCommand = new(AddEmployee);
            EditEmployeeCommand = new(EditEmployee);
            RemoveEmployeeCommand = new(RemoveEmployee);
            Text = "Идет процесс создания списка сотрудников. Пожалуйста подождите.";
            LoadingEmployeeList();
            ListenToServer();
        }

        private async void ListenToServer() {
            try {
                while (true) {
                    byte[] buffer = await _server.ReadFromStream(1);
                    byte message = buffer[0];

                    if (message == Message.EmployeeList) {
                        buffer = await _server.ReadFromStream(4);
                        buffer = await _server.ReadFromStream(BitConverter.ToInt32(buffer, 0));

                        var employees = JsonSerializer.Deserialize<EmployeeDTO[]>(Encoding.UTF8.GetString(buffer))
                            .Select(employeeDTO => Employee.FromDTO(employeeDTO));

                        Employees.Clear();
                        foreach (var employee in employees)
                            Employees.Add(employee);
                        IsEnabledInterface = true;
                        VisibilityProcess = Visibility.Hidden;
                    }
                    else if (message == Message.DataSaveSuccess) {
                        buffer = await _server.ReadFromStream(4);
                        buffer = await _server.ReadFromStream(BitConverter.ToInt32(buffer, 0));
                        MessageBox.Show(Encoding.UTF8.GetString(buffer));
                        IsEnabledInterface = true;
                        VisibilityProcess = Visibility.Hidden;
                        Text = "Идет процесс обновления списка сотрудников. Пожалуйста подождите.";
                        LoadingEmployeeList();
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

        private async void LoadingEmployeeList() {
            IsEnabledInterface = false;
            VisibilityProcess = Visibility.Visible;
            await SendMessageServer.SendEmployeeListMessage(_server);
        }

        private void BreakConnection() {
            if (_server.Client.Connected)
                _server.Client.Shutdown(SocketShutdown.Both);
            _server.Client.Close();
        }

        private async void AddEmployee() {

            AddEditEmployeeWindow dialog = new();
            if (dialog.ShowDialog() == true) {
                IsEnabledInterface = false;
                VisibilityProcess = Visibility.Visible;
                Employee employee = dialog.ViewModel.Employee;
                EmployeeDataVerification(employee);
                Text = "Идет процесс добавления нового сотрудника. Пожалуйста подождите.";
                await SendMessageServer.SendAddNewEmployeeMessage(_server, employee);
            }
        }

        private async void EditEmployee() {
            if (SelectedEmployee is null) 
                return;
            AddEditEmployeeWindow dialog = new(SelectedEmployee);
            if (dialog.ShowDialog() == true) {
                Employee employee = dialog.ViewModel.Employee;
                EmployeeDataVerification(employee);
                IsEnabledInterface = false;
                VisibilityProcess = Visibility.Visible;
                Text = "Идет процесс изменения данных сотрудника. Пожалуйста подождите.";
                await SendMessageServer.SendEditEmployeeMessage(_server, employee);
            }
        }

        private async void RemoveEmployee() {
            if (SelectedEmployee is null)
                return;

            IsEnabledInterface = false;
            VisibilityProcess = Visibility.Visible;
            Text = "Идет процесс удаления сотрудника. Пожалуйста подождите.";
            await SendMessageServer.SendRemoveEmployeeMessage(_server, SelectedEmployee);
        }

        private void EmployeeDataVerification(Employee employee) {
            Employee currentEmployee = employee;
            while (true) {
                if (Employees.Any(e => e.PhoneNumber == currentEmployee.PhoneNumber && e.Id != currentEmployee.Id))
                    MessageBox.Show("Такой телефонный номер уже есть в базе!");
                else if (Employees.Any(e => e.Email == currentEmployee.Email && e.Id != currentEmployee.Id))
                    MessageBox.Show("Такой email уже есть в базе!");
                else if (Employees.Any(e => e.Login == currentEmployee.Login && e.Id != currentEmployee.Id))
                    MessageBox.Show("Такой логин уже есть в базе!");
                else
                    return;
                AddEditEmployeeWindow dialog = new(currentEmployee);
                if (dialog.ShowDialog() == true) {
                    currentEmployee = dialog.ViewModel.Employee;
                }

            }
        }

    }
}
