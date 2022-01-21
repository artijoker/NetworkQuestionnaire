using System;
using System.Collections.Generic;

namespace Library {
    public partial class Employee {

        public int Id { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Patronymic { get; set; }
        public DateTime BirthDate { get; set; }
        public string Email { get; set; }
        

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
