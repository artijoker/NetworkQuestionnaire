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
        private static Regex _pattern = new("^7[0-9]{11}");
        private int _id;
        private string _login;
        private string _password;
        private string _name;
        private string _surname;
        private string _patronymic;
        private DateTime _birthDate;
        private string _phoneNumber;
        private string _email;
        private bool _isDialogResult;

        public event PropertyChangedEventHandler PropertyChanged;

        public Employee Employee { get; set; }

        public bool IsDialogResult {
            get => _isDialogResult;
            set {
                _isDialogResult = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsDialogResult)));
            }
        }

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

        public string Name {
            get => _name;
            set {
                _name = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Name)));
            }
        }

        public string Surname {
            get => _surname;
            set {
                _surname = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Surname)));
            }
        }

        public string Patronymic {
            get => _patronymic;
            set {
                _patronymic = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Patronymic)));
            }
        }

        public DateTime BirthDate {
            get => _birthDate;
            set {
                _birthDate = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(BirthDate)));
            }
        }

        public string PhoneNumber {
            get => _phoneNumber;
            set {
                _phoneNumber = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(PhoneNumber)));
            }
        }

        public string Email {
            get => _email;
            set {
                _email = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Email)));
            }
        }

        public DelegateCommand SaveCommand { get; }
        public DelegateCommand CancelCommand { get; }

        public DelegateCommand CreatePasswordCommand { get; }

        public AddEditEmployeeWindowViewModel() {
            _id = 0;
            Employee = new Employee();
            BirthDate = DateTime.Now.Date;
            SaveCommand = new(Save);
            CancelCommand = new(Cancel);
            CreatePasswordCommand = new(() => Password = MakePassword());
        }

        public AddEditEmployeeWindowViewModel(Employee employee) {
            _id = employee.Id;
            Name = employee.Name;
            Surname = employee.Surname;
            Patronymic = employee.Patronymic;
            BirthDate = employee.BirthDate;
            PhoneNumber = employee.PhoneNumber;
            Email = employee.Email;
            Login = employee.Login;
            Password = employee.Password;
            Employee = new Employee();
            SaveCommand = new(Save);
            CancelCommand = new(Cancel);
            CreatePasswordCommand = new(() => Password = MakePassword());
        }

        private void Save() {
            if (string.IsNullOrEmpty(Name)) {
                MessageBox.Show("Введите имя сотрудника!");
                return;
            }
            if (string.IsNullOrEmpty(Surname)) {
                MessageBox.Show("Введите фамилию сотрудника!");
                return;
            }
            if (BirthDate.Date == DateTime.Now.Date) {
                MessageBox.Show("Введите дату рождения сотрудника!");
                return;
            }
            if (string.IsNullOrEmpty(PhoneNumber)) {
                MessageBox.Show("Введите номер телефона сотрудника!");
                return;
            }
            if (string.IsNullOrEmpty(Email)) {
                MessageBox.Show("Введите email сотрудника!");
                return;
            }
            if (string.IsNullOrEmpty(Login)) {
                MessageBox.Show("Введите логин сотрудника!");
                return;
            }
            if (string.IsNullOrEmpty(Password)) {
                MessageBox.Show("Введите пароль сотрудника!");
                return;
            }
            
            if (_pattern.IsMatch(PhoneNumber)) {
                MessageBox.Show("Телефонный номер указан неверно!\n Введите номер телофона в формате 7**********. Где * - цифра а 7 - это код старны");
                return;
            }
            Employee.Id = _id;
            Employee.Name = Name;
            Employee.Surname = Surname;
            Employee.Patronymic = Patronymic;
            Employee.BirthDate = BirthDate;
            Employee.PhoneNumber = PhoneNumber;
            Employee.Email = Email;
            Employee.Login = Login;
            Employee.Password = Password;

            IsDialogResult = true;
        }
        private void Cancel() => IsDialogResult = false;


        private string MakePassword() {
            Random generator = new();
            string symbols = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789№%*_@#$&";
            StringBuilder builder = new();
            int length = generator.Next(8, 10);
            for (int i = 0; i < length; i++) {
                char symbol = symbols[generator.Next(symbols.Length)];
                builder.Append(symbol);
            }
            return builder.ToString();
        }
    }
}
