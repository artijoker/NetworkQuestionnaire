using Library;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdminClient {
    public class AddEditSurveyWindowViewModel : INotifyPropertyChanged {
        private bool _isDialogResult;
        private string _textSurvey;
        private Question _selectedQuestion;
        private QuestionType[] _questionTypes;

        public event PropertyChangedEventHandler PropertyChanged;
        public Survey Survey { get; }
        public ObservableCollection<Question> Questions { get; }

        public bool IsDialogResult {
            get => _isDialogResult;
            set {
                _isDialogResult = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsDialogResult)));
            }
        }

        public string TextSurvey {
            get => _textSurvey;
            set {
                _textSurvey = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(TextSurvey)));
            }
        }

        public Question SelectedQuestion {
            get => _selectedQuestion;
            set {
                _selectedQuestion = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SelectedQuestion)));
            }
        }
        public DelegateCommand AddQuestionCommand { get; }
        public DelegateCommand EditQuestionCommand { get; }
        public DelegateCommand RemoveQuestionCommand { get; }

        public DelegateCommand SaveCommand { get; }
        public DelegateCommand CancelCommand { get; }

        public AddEditSurveyWindowViewModel(QuestionType[] questionTypes) {
            Survey = new Survey();
            _questionTypes = questionTypes;
            Questions = new();
            SaveCommand = new(Save);
            CancelCommand = new(Cancel);
        }

        public AddEditSurveyWindowViewModel(QuestionType[] questionTypes, Survey survey) {
            _questionTypes = questionTypes;
            Survey = survey;
            TextSurvey = Survey.Name;
            Questions = new(Survey.Questions);
            SaveCommand = new(Save);
            CancelCommand = new(Cancel);
        }

        private void Save() => IsDialogResult = true;
        private void Cancel() => IsDialogResult = false;
    }
}
