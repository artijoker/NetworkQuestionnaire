﻿using Library;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace UserClient {
    public class SurveyWindowViewModel : INotifyPropertyChanged {

        private bool _isReady;
        public EmployeeSurveyAnswerDTO EmployeeSurveyAnswer { get; }

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

        public SurveyWindowViewModel(Survey survey) {

            //string jsonString = JsonSerializer.Serialize(survey.ToDTO());
            //File.WriteAllText("Survey.json", jsonString);

            SurveyName = survey.Name;
            EmployeeSurveyAnswer = new EmployeeSurveyAnswerDTO();
            ReadyCommand = new(Ready);
            QuestionViewModels = new ObservableCollection<QuestionViewModel.QuestionViewModel>(
               survey.Questions.Select(question => question.GetViewModel()));

        }

        private void Ready() {
            foreach (var questionViewModel in QuestionViewModels)
                if (questionViewModel.IsRequired)
                    if (!questionViewModel.IsThereAnswer()) {
                        MessageBox.Show("Вы ответили не на все обязательные вопросы!");
                        return;
                    }

            //if (QuestionViewModels.Where(q => q.IsRequired).All(q => q.IsThereAnswer())) {
            //    MessageBox.Show("Вы ответили не на все вопросы!");
            //    return;
            //}

            foreach (var questionViewModel in QuestionViewModels)
                questionViewModel.SaveAnswerEmployee(EmployeeSurveyAnswer);
            
            IsReady = true;
        }
    }
}