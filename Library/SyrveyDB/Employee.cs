using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Library {
    public partial class Employee : INotifyPropertyChanged {
        private int _id;
        private string _login;
        private string _password;
        private string _name;
        private string _surname;
        private string _patronymic;
        private DateTime _birthDate;
        private string _email;

        public event PropertyChangedEventHandler PropertyChanged;

        public int Id {
            get => _id;
            set {
                _id = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Id)));
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


        public string Email {
            get => _email;
            set {
                _email = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Email)));
            }
        }


        public ICollection<Survey> Surveys { get; set; } = new HashSet<Survey>();
        public ICollection<EmployeeFreeAnswer> EmployeeFreeAnswers { get; set; } = new HashSet<EmployeeFreeAnswer>();
        public ICollection<SingleAnswer> SingleAnswers { get; set; } = new HashSet<SingleAnswer>();
        public ICollection<MultipleAnswer> MultipleAnswers { get; set; } = new HashSet<MultipleAnswer>();

        public EmployeeDTO ToDTO() =>
            new EmployeeDTO() { 
                Id = Id, 
                Login = Login, 
                Password = Password, 
                Name = Name, 
                Surname = Surname, 
                Patronymic = Patronymic,
                BirthDate = BirthDate,
                Email = Email 
            };

        static public Employee FromDTO(EmployeeDTO employeeDTO) =>
            new Employee() {
                Id = employeeDTO.Id, 
                Login = employeeDTO.Login, 
                Password = employeeDTO.Password, 
                Name = employeeDTO.Name, 
                Surname = employeeDTO.Surname, 
                Patronymic = employeeDTO.Patronymic,
                BirthDate = employeeDTO.BirthDate,
                Email = employeeDTO.Email 
            };
    }
}
