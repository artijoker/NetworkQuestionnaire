using Library;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdminClient {
    class EmployeeAndAnswersWindowViewModel : INotifyPropertyChanged {

        private bool _isReady;

        public string SurveyName { get; }
        public ObservableCollection<QuestionViewModel.QuestionViewModel> QuestionViewModels { get; }

        public event PropertyChangedEventHandler PropertyChanged;

        public bool IsReady {
            get => _isReady;
            set {
                _isReady = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsReady)));
            }
        }

        public DelegateCommand ReadyCommand { get; }

        public EmployeeAndAnswersWindowViewModel(Survey survey) {

            SurveyName = survey.Name;
            ReadyCommand = new(() => IsReady = true);
            QuestionViewModels = new ObservableCollection<QuestionViewModel.QuestionViewModel>(
               survey.Questions.Select(question => QuestionViewModel.QuestionViewModel.GetViewModel(question)));

            int x = 5;
        }

    }
}
