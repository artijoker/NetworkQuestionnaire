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
        public DelegateCommand ShowCompletedSurveysCommand { get; }

        public EmployeesWindowViewModel(TcpClient server) {
            _server = server;
            IsEnabledInterface = true;
            VisibilityProcess = Visibility.Hidden;
            AddEmployeeCommand = new(AddEmployee);
            EditEmployeeCommand = new(EditEmployee);
            RemoveEmployeeCommand = new(RemoveEmployee);
            ShowCompletedSurveysCommand = new(ShowCompletedSurveys);
            Text = "Идет процесс загрузки списка сотрудников. Пожалуйста подождите.";
            LoadingEmployeeList();
            ListenToServer();
        }

        private async void ListenToServer() {
            try {
                while (true) {
                    byte[] buffer = await _server.ReadFromStream(1);
                    byte message = buffer[0];

                    if (message == Message.EmployeesList) {
                        buffer = await _server.ReadFromStream(4);
                        buffer = await _server.ReadFromStream(BitConverter.ToInt32(buffer, 0));

                        var employees = JsonSerializer.Deserialize<EmployeeDTO[]>(Encoding.UTF8.GetString(buffer))
                            .Select(employeeDTO => Employee.FromDTO(employeeDTO));

                        Employees.Clear();
                        foreach (var employee in employees)
                            Employees.Add(employee);
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
                        Text = "Идет процесс обновления списка сотрудников. Пожалуйста подождите.";
                        LoadingEmployeeList();
                    }
                    else if (message == Message.SurveysСompletedEmployee) {
                        buffer = await _server.ReadFromStream(4);
                        buffer = await _server.ReadFromStream(BitConverter.ToInt32(buffer, 0));

                        Survey[] surveys = JsonSerializer.Deserialize<SurveyDTO[]>(Encoding.UTF8.GetString(buffer))
                            .Select(surveyDTO => Survey.FromDTO(surveyDTO)).ToArray();


                        VisibilityProcess = Visibility.Hidden;
                        if (surveys.Length == 0) {
                            MessageBox.Show(
                                "У сотридника нет пройденных опросов!",
                                "Загрузка завершена",
                                MessageBoxButton.OK,
                                MessageBoxImage.Information
                                );
                        }
                        else {
                            EmployeeAndSurveysWindow dialog = new(surveys);
                            dialog.ShowDialog();
                        }
                        IsEnabledInterface = true;
                    }

                }
            }
            catch (Exception ex) {
                IsEnabledInterface = true;
                VisibilityProcess = Visibility.Hidden;
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
            Employee employee;
            
                AddEditEmployeeWindow dialog = new();
            while (true) {
                if (dialog.ShowDialog() == true) {
                    employee = dialog.ViewModel.Employee;
                    if (!IsEmailCorrect(employee)) {
                        MessageBox.Show("Сотрудник с таким email уже есть в базе!",
                            "Ошибка!",
                            MessageBoxButton.OK,
                            MessageBoxImage.Error
                            );
                        dialog = new(employee);
                        continue;
                    }
                    if (!IsLoginCorrect(employee)) {
                        MessageBox.Show("Сотрудник с таким логином уже есть в базе!",
                            "Ошибка!",
                            MessageBoxButton.OK,
                            MessageBoxImage.Error
                            );
                        dialog = new(employee);
                        continue;
                    }
                    IsEnabledInterface = false;
                    VisibilityProcess = Visibility.Visible;
                    Text = "Идет процесс сохранения нового сотрудника. Пожалуйста подождите.";
                    await SendMessageServer.SendAddNewEmployeeMessage(_server, employee);
                    break;
                }
                else
                    break;
            }
        }



        private async void EditEmployee() {
            if (SelectedEmployee is null)
                return;
            Employee employee = SelectedEmployee;

            AddEditEmployeeWindow dialog = new(employee);
            while (true) {
                if (dialog.ShowDialog() == true) {
                    employee = dialog.ViewModel.Employee;
                    if (!IsEmailCorrect(employee)) {
                        MessageBox.Show("Сотрудник с таким email уже есть в базе!",
                            "Ошибка!",
                            MessageBoxButton.OK,
                            MessageBoxImage.Error
                            );
                        dialog = new(employee);
                        continue;
                    }
                    if (!IsLoginCorrect(employee)) {
                        MessageBox.Show("Сотрудник с таким логином уже есть в базе!",
                            "Ошибка!",
                            MessageBoxButton.OK,
                            MessageBoxImage.Error
                            );
                        dialog = new(employee);
                        continue;
                    }
                    IsEnabledInterface = false;
                    VisibilityProcess = Visibility.Visible;
                    Text = "Идет процесс изменения данных сотрудника. Пожалуйста подождите.";
                    await SendMessageServer.SendEditEmployeeMessage(_server, employee);
                    break;
                }
                else
                    break;
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


        private async void ShowCompletedSurveys() {
            if (SelectedEmployee is null)
                return;
            IsEnabledInterface = false;
            VisibilityProcess = Visibility.Visible;
            Text = "Идет процесс загрузки всех опросов которые прошел сотрудник. Пожалуйста подождите.";
            await SendMessageServer.SendAllAnswersEmployeeMessage(_server, SelectedEmployee);
        }

        private bool IsEmailCorrect(Employee employee) =>
            !Employees.Any(e => e.Email == employee.Email && e.Id != employee.Id);

        private bool IsLoginCorrect(Employee employee) =>
            !Employees.Any(e => e.Login == employee.Login && e.Id != employee.Id);

    }
}
