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
            SelectedQuestionType = questionTypes[0];
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
            SaveAnswerId(question);
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
            if (string.IsNullOrEmpty(TextQuestion)) {
                MessageBox.Show(
                    "Введите текст вопроса!",
                    "Внимание",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning
                );
                return;
            }
            if (Answers.Count == 0 && SelectedQuestionType.Type != "Free") {
                MessageBox.Show(
                    "Нельзя сохранять вопрос если в нем отсутствуют ответы и его тип 'Один ответ из списка' или 'Несколько ответов из списка!",
                    "Внимание",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning
                );
                return;
            }
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

            if (SelectedQuestionType.Type == "Free" && Question.FreeAnswers.Count == 0) {
                Question.FreeAnswers.Add(new FreeAnswer() { Id = 0, QuestionId = _questionId });
                Question.MultipleAnswers.Clear();
                Question.SingleAnswers.Clear();
            }
            if (SelectedQuestionType.Type == "Single") {
                if (Question.SingleAnswers.Count > 0) {
                    List<SingleAnswer> singleAnswers = Question.SingleAnswers.ToList();
                    for (int i = 0; i < Answers.Count; i++) {
                        if (i <= singleAnswers.Count - 1)
                            singleAnswers[i].Text = Answers[i];
                        else
                            singleAnswers.Add(new() { Id = 0, QuestionId = _questionId, Text = Answers[i] });
                    }
                    Question.SingleAnswers = singleAnswers.GetRange(0, Answers.Count);
                }
                else {
                    foreach (var answer in Answers) 
                            Question.SingleAnswers.Add(new() { Id = 0, QuestionId = _questionId, Text = answer });
                    
                    Question.FreeAnswers.Clear();
                    Question.MultipleAnswers.Clear();
                }
            }
            if (SelectedQuestionType.Type == "Multiple") {
                if (Question.MultipleAnswers.Count > 0) {
                    List<MultipleAnswer> multipleAnswers = Question.MultipleAnswers.ToList();
                    for (int i = 0; i < Answers.Count; i++) {
                        if (i <= multipleAnswers.Count - 1)
                            multipleAnswers[i].Text = Answers[i];
                        else
                            multipleAnswers.Add(new() { Id = 0, QuestionId = _questionId, Text = Answers[i] });
                    }
                    Question.MultipleAnswers = multipleAnswers.GetRange(0, Answers.Count);
                }
                else {
                    foreach (var answer in Answers)
                            Question.MultipleAnswers.Add(new() { Id = 0, QuestionId = _questionId, Text = answer });
                    
                    Question.FreeAnswers.Clear();
                    Question.SingleAnswers.Clear();
                }
            }


        }

        private void AddAnswer() {
            IsEnabledInterface = false;
            VisibilityInputField = Visibility.Visible;
            SelectedAnswer = null;
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
            if (SelectedQuestionType.Type == "Single") {
                if (Question.SingleAnswers.Count > 0) {
                    int index = Answers.IndexOf(SelectedAnswer);
                    if (index < Question.SingleAnswers.Count - 1)
                        Question.SingleAnswers.Remove(Question.SingleAnswers.ElementAt(index));
                }
            }

            if (SelectedQuestionType.Type == "Multiple") {
                if (Question.MultipleAnswers.Count > 0) {
                    int index = Answers.IndexOf(SelectedAnswer);
                    if (index < Question.MultipleAnswers.Count - 1)
                        Question.MultipleAnswers.Remove(Question.MultipleAnswers.ElementAt(index));
                }
            }

            Answers.Remove(SelectedAnswer);
        }

        private void OKInputField() {
            if (string.IsNullOrEmpty(TextAnswer))
                return;

            if (SelectedAnswer is not null) {
                int idx = 0;
                for (int i = 0; i < Answers.Count; i++) {
                    if ((Object)Answers[i] == (Object)SelectedAnswer) {
                        idx = i;
                    }
                }
                //nt idx = Answers.IndexOf(SelectedAnswer);
                Answers[idx] = TextAnswer.Trim();
            }
            else
                Answers.Add(TextAnswer.Trim());

            TextAnswer = "";

            IsEnabledInterface = true;
            VisibilityInputField = Visibility.Hidden;
        }
        private void CancelInputField() {
            IsEnabledInterface = true;
            VisibilityInputField = Visibility.Hidden;
            TextAnswer = "";
        }

        private void SaveAnswerId(Question question) {
            if (question.FreeAnswers.Count == 1) 
                Question.FreeAnswers.Add(new FreeAnswer() { Id = question.FreeAnswers.Single().Id, QuestionId = question.Id });
            if (question.SingleAnswers.Count > 0) 
                foreach (var answer in question.SingleAnswers) 
                    Question.SingleAnswers.Add(new SingleAnswer() { Id = answer.Id, QuestionId = question.Id });
            if (question.MultipleAnswers.Count > 0)
                foreach (var answer in question.MultipleAnswers)
                    Question.MultipleAnswers.Add(new MultipleAnswer() { Id = answer.Id, QuestionId = question.Id });
        }
    }
}
