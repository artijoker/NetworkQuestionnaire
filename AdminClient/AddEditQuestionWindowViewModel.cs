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
    public class AddEditQuestionWindowViewModel : INotifyPropertyChanged {

        private bool _isDialogResult;
        private bool _isRequired;
        private bool _isEnabledInterface;
        private int _questionId;
        private string _textQuestion;
        private string _textAnswer;
        private string _selectedAnswer;
        private QuestionType _selectedQuestionType;
        private Visibility _visibilityInputField;
        private Visibility _visibilityAnswersList;
        public QuestionType[] QuestionTypes { get; }

        public event PropertyChangedEventHandler PropertyChanged;
        public Question Question { get; }
        public ObservableCollection<string> Answers { get; }

        public bool IsDialogResult {
            get => _isDialogResult;
            set {
                _isDialogResult = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsDialogResult)));
            }
        }

        public bool IsRequired {
            get => _isRequired;
            set {
                _isRequired = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsRequired)));
            }
        }

        public bool IsEnabledInterface {
            get => _isEnabledInterface;
            set {
                _isEnabledInterface = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsEnabledInterface)));
            }
        }

        public string TextQuestion {
            get => _textQuestion;
            set {
                _textQuestion = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(TextQuestion)));
            }
        }
        public string TextAnswer {
            get => _textAnswer;
            set {
                _textAnswer = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(TextAnswer)));
            }
        }

        public string SelectedAnswer {
            get => _selectedAnswer;
            set {
                _selectedAnswer = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SelectedAnswer)));
            }
        }

        public QuestionType SelectedQuestionType {
            get => _selectedQuestionType;
            set {
                _selectedQuestionType = value;
                if (SelectedQuestionType.Type == "Free")
                    VisibilityAnswersList = Visibility.Hidden;
                else
                    VisibilityAnswersList = Visibility.Visible;

                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SelectedQuestionType)));
            }
        }

        public Visibility VisibilityInputField {
            get => _visibilityInputField;
            set {
                _visibilityInputField = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(VisibilityInputField)));
            }
        }

        public Visibility VisibilityAnswersList {
            get => _visibilityAnswersList;
            set {
                _visibilityAnswersList = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(VisibilityAnswersList)));
            }
        }


        public DelegateCommand AddAnswerCommand { get; }
        public DelegateCommand EditAnswerCommand { get; }
        public DelegateCommand RemoveAnswerCommand { get; }

        public DelegateCommand SaveCommand { get; }
        public DelegateCommand CancelCommand { get; }

        public DelegateCommand OKInputFieldCommand { get; }
        public DelegateCommand CancelInputFieldCommand { get; }

        public AddEditQuestionWindowViewModel(QuestionType[] questionTypes) {
            QuestionTypes = questionTypes;
            _questionId = 0;
            Answers = new();
            Question = new();
            SaveCommand = new(Save);
            CancelCommand = new(Cancel);
            AddAnswerCommand = new(AddAnswer);
            EditAnswerCommand = new(EditAnswer);
            RemoveAnswerCommand = new(RemoveAnswer);
            OKInputFieldCommand = new(OKInputField);
            CancelInputFieldCommand = new(CancelInputField);
            IsEnabledInterface = true;
            VisibilityAnswersList = Visibility.Visible;
            VisibilityInputField = Visibility.Hidden;
        }
        public AddEditQuestionWindowViewModel(QuestionType[] questionTypes, Question question) {
            Question = new();
            QuestionTypes = questionTypes;
            TextQuestion = question.Text;
            _questionId = question.Id;
            IsRequired = question.IsRequired;
            SelectedQuestionType = question.Type;
            SaveCommand = new(Save);
            CancelCommand = new(Cancel);
            AddAnswerCommand = new(AddAnswer);
            EditAnswerCommand = new(EditAnswer);
            RemoveAnswerCommand = new(RemoveAnswer);
            OKInputFieldCommand = new(OKInputField);
            CancelInputFieldCommand = new(CancelInputField);
            IsEnabledInterface = true;
            VisibilityInputField = Visibility.Hidden;
            Answers = new();
            if (question.MultipleAnswers.Count != 0)
                foreach (var answer in question.MultipleAnswers)
                    Answers.Add(answer.Text);
            if (question.SingleAnswers.Count != 0)
                foreach (var answer in question.SingleAnswers)
                    Answers.Add(answer.Text);

        }

        private void Save() {
            if (Answers.Count == 0 && SelectedQuestionType.Type != "Free")
                return;
            SaveQuestion();
            IsDialogResult = true;
        }
        private void Cancel() => IsDialogResult = false;


        private void SaveQuestion() {
            Question.Id = _questionId;
            Question.Text = TextQuestion.Trim();
            Question.IsRequired = IsRequired;
            Question.QuestionTypeId = SelectedQuestionType.Id;
            Question.Type = SelectedQuestionType;

            if (SelectedQuestionType.Type == "Free")
                Question.FreeAnswers.Add(new FreeAnswer() { Id = 0, QuestionId = _questionId });
            else {
                foreach (var answer in Answers) {
                    if (SelectedQuestionType.Type == "Single")
                        Question.SingleAnswers.Add(new() { Id = 0, QuestionId = _questionId, Text = answer });
                    else if (SelectedQuestionType.Type == "Multiple")
                        Question.MultipleAnswers.Add(new() { Id = 0, QuestionId = _questionId, Text = answer });
                    else
                        throw new FormatException();
                }
            }

        }

        private void AddAnswer() {
            IsEnabledInterface = false;
            VisibilityInputField = Visibility.Visible;
        }

        private void EditAnswer() {
            if (SelectedAnswer is null)
                return;
            TextAnswer = SelectedAnswer;
            IsEnabledInterface = false;
            VisibilityInputField = Visibility.Visible;

        }

        private void RemoveAnswer() {
            if (SelectedAnswer is null)
                return;
            Answers.Remove(SelectedAnswer);
        }

        private void OKInputField() {
            IsEnabledInterface = true;
            VisibilityInputField = Visibility.Hidden;

            if (string.IsNullOrEmpty(TextAnswer))
                return;
            if (SelectedAnswer is null) {
                Answers[Answers.IndexOf(SelectedAnswer)] = TextAnswer;
            }
            Answers.Add(TextAnswer);

        }
        private void CancelInputField() {
            IsEnabledInterface = true;
            VisibilityInputField = Visibility.Hidden;
            TextAnswer = "";
        }
    }
}
