using Library;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserClient.QuestionViewModel {
    public class RadioButtonViewModel : INotifyPropertyChanged {
        private bool _isChecked = false;

        public SingleAnswer Answer { get; }
        public int GroupId { get; }

        public event PropertyChangedEventHandler PropertyChanged;

        public bool IsChecked {
            get => _isChecked;
            set {
                _isChecked = value;
                if (_isChecked == value) return;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsChecked)));
            }
        }

        public RadioButtonViewModel(SingleAnswer answer, int groupId) {
            Answer = answer;
            GroupId = groupId;
        }


    }
}
