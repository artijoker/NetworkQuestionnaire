using Library;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserClient {
    public class CheckBoxViewModel : INotifyPropertyChanged {
        private bool _isChecked = false;

        public MultipleAnswer Answer { get; }

        public event PropertyChangedEventHandler PropertyChanged;

        public bool IsChecked {
            get => _isChecked;
            set {
                _isChecked = value;
                if (_isChecked == value) return;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsChecked)));
            }
        }

        public CheckBoxViewModel(MultipleAnswer answer) {
            Answer = answer;
        }
        

    }
}
