using Library;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace AdminClient {
    public class AddEditSurveyWindowViewModel : INotifyPropertyChanged {
        private bool _isDialogResult;
        private int _surveyId;
        private string _textSurvey;
        private Question _selectedQuestion;
        private readonly QuestionType[] _questionTypes;

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
            _surveyId = 0;
            _questionTypes = questionTypes;
            Questions = new();
            SaveCommand = new(Save);
            CancelCommand = new(Cancel);
            AddQuestionCommand = new(AddQuestion);
            EditQuestionCommand = new(EditQuestion);
            RemoveQuestionCommand = new(RemoveQuestion);
        }

        public AddEditSurveyWindowViewModel(QuestionType[] questionTypes, Survey survey) {
            Survey = new Survey();
            _surveyId = survey.Id;
            _questionTypes = questionTypes;
            TextSurvey = survey.Name;
            Questions = new(survey.Questions);
            SaveCommand = new(Save);
            CancelCommand = new(Cancel);
            AddQuestionCommand = new(AddQuestion);
            EditQuestionCommand = new(EditQuestion);
            RemoveQuestionCommand = new(RemoveQuestion);
        }

        private void AddQuestion() {
            AddEditQuestionWindow dialog = new(_questionTypes);
            if (dialog.ShowDialog() == true) {
                Question question = dialog.ViewModel.Question;
                question.SurveyId = _surveyId;
                Questions.Add(dialog.ViewModel.Question);
            }

        }

        private void EditQuestion() {
            AddEditQuestionWindow dialog = new(_questionTypes, SelectedQuestion);
            if (dialog.ShowDialog() == true) {
                Question question = dialog.ViewModel.Question;
                question.SurveyId = _surveyId;
                Questions[Questions.IndexOf(SelectedQuestion)] = dialog.ViewModel.Question;
            }
            SelectedQuestion = null;
        }

        private void RemoveQuestion() {
            if (SelectedQuestion is null)
                return;
            Questions.Remove(SelectedQuestion);
        }

        private void Save() {
            if (string.IsNullOrEmpty(TextSurvey)) {
                MessageBox.Show(
                    "Введите название опроса!",
                    "Внимание",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning
                );
                return;
            }

            if (Questions.Count == 0) {
                MessageBox.Show(
                    "Этот опрос нельзя сохранить так как в нем отсутствуют вопросы!",
                    "Внимание",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning
                );
                return;
            }
            SaveSurvey();
            IsDialogResult = true;
        }
        private void Cancel() => IsDialogResult = false;


        private void SaveSurvey() {
            Survey.Id = _surveyId;
            Survey.Name = TextSurvey.Trim();
            Survey.Questions = Questions.ToList();
        }
    }
}
