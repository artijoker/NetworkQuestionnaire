using Library;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace AdminClient {
    class EmployeeAndSurveysWindowViewModel : INotifyPropertyChanged {
        private bool _isHide;
        private Survey _selectedSurvey;
        private bool _isDialogResult;

        public event PropertyChangedEventHandler PropertyChanged;

        public ObservableCollection<Survey> Surveys { get; }

        public bool IsHide {
            get => _isHide;
            set {
                _isHide = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsHide)));
            }
        }

        public Survey SelectedSurvey {
            get => _selectedSurvey;
            set {
                _selectedSurvey = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SelectedSurvey)));
            }
        }

        public bool IsDialogResult {
            get => _isDialogResult;
            set {
                _isDialogResult = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsDialogResult)));
            }
        }

        public DelegateCommand ShowCommand { get; }
        public DelegateCommand BackCommand { get; }

        public EmployeeAndSurveysWindowViewModel(Survey[] surveys) {
            Surveys = new(surveys);
            ShowCommand = new(Show);
            BackCommand = new(Back);
        }

        private void Show() {
            if (SelectedSurvey is null)
                return;
            EmployeeAndAnswersWindow dialog = new(SelectedSurvey);
            dialog.ShowDialog();
        }

        private void Back() => IsDialogResult = false;
    }
}
