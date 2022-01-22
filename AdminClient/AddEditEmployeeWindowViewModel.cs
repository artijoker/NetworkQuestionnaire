using Library;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;

namespace AdminClient {
    public class AddEditEmployeeWindowViewModel : INotifyPropertyChanged {
        private bool _isDialogResult;

        public event PropertyChangedEventHandler PropertyChanged;

        public bool IsDialogResult {
            get => _isDialogResult;
            set {
                _isDialogResult = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsDialogResult)));
            }
        }

        public Employee Employee {get; set; }


        public DelegateCommand SaveCommand { get; }
        public DelegateCommand CancelCommand { get; }

        public DelegateCommand CreatePasswordCommand { get; }

        public AddEditEmployeeWindowViewModel() {
            Employee = new Employee();
            Employee.Id = 0;
            Employee.BirthDate = DateTime.Now.Date;
            SaveCommand = new(Save);
            CancelCommand = new(Cancel);
            CreatePasswordCommand = new(() => Employee.Password = MakePassword());
        }

        public AddEditEmployeeWindowViewModel(Employee employee) {
            Employee = new Employee();
            Employee.Id = employee.Id;
            Employee.Name = employee.Name;
            Employee.Surname = employee.Surname;
            Employee.Patronymic = employee.Patronymic;
            Employee.BirthDate = employee.BirthDate;
            Employee.Email = employee.Email;
            Employee.Login = employee.Login;
            Employee.Password = employee.Password;
            
            SaveCommand = new(Save);
            CancelCommand = new(Cancel);
            CreatePasswordCommand = new(() => Employee.Password = MakePassword());
        }

        private void Save() {
            if (string.IsNullOrEmpty(Employee.Name)) {
                MessageBox.Show(
                    "Введите имя сотрудника!",
                    "Внимание",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning
                );
                return;
            }
            if (string.IsNullOrEmpty(Employee.Surname)) {
                MessageBox.Show(
                    "Введите фамилию сотрудника!",
                    "Внимание",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning
                );
                return;
            }
            if (Employee.BirthDate.Date > DateTime.Now.Date) {
                MessageBox.Show(
                   "Нельзя указывать будущую дату!",
                   "Внимание",
                   MessageBoxButton.OK,
                   MessageBoxImage.Warning
               );
                return;
            }
            if (string.IsNullOrEmpty(Employee.Email)) {
                MessageBox.Show(
                   "Введите email сотрудника!",
                   "Внимание",
                   MessageBoxButton.OK,
                   MessageBoxImage.Warning
               );
                return;
            }
            if (string.IsNullOrEmpty(Employee.Login)) {
                MessageBox.Show(
                   "Введите логин сотрудника!",
                   "Внимание",
                   MessageBoxButton.OK,
                   MessageBoxImage.Warning
               );
                return;
            }
            if (string.IsNullOrEmpty(Employee.Password)) {
                MessageBox.Show(
                   "Введите пароль сотрудника!",
                   "Внимание",
                   MessageBoxButton.OK,
                   MessageBoxImage.Warning
               );
                return;
            }
            IsDialogResult = true;
        }
        private void Cancel() => IsDialogResult = false;


        private string MakePassword() {
            Random generator = new();
            string symbols = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789№%*_@#$&";
            StringBuilder builder = new();
            int length = generator.Next(10, 12);
            for (int i = 0; i < length; i++) {
                char symbol = symbols[generator.Next(symbols.Length)];
                builder.Append(symbol);
            }
            return builder.ToString();
        }
    }
}
