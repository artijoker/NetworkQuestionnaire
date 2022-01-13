using Library;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdminClient {
    public class AddEditEmployeeWindowViewModel : INotifyPropertyChanged {
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

        public DelegateCommand SaveCommand { get; }
        public DelegateCommand CancelCommand { get; }


        public AddEditEmployeeWindowViewModel() {
            Employee = new Employee();
            SaveCommand = new(Save);
            CancelCommand = new(Cancel);
        }

        public AddEditEmployeeWindowViewModel(Employee employee) {
            Employee = employee.Clone();
            SaveCommand = new(Save);
            CancelCommand = new(Cancel);
        }

        private void Save() => IsDialogResult = true;
        private void Cancel() => IsDialogResult = false;
    }
}
