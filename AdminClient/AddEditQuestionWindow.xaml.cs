using Library;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace AdminClient {
    /// <summary>
    /// Логика взаимодействия для AddEditQuestionWindow.xaml
    /// </summary>
    public partial class AddEditQuestionWindow : Window {
        public AddEditQuestionWindowViewModel ViewModel { get; }

        public AddEditQuestionWindow(QuestionType[] questionTypes) {
            InitializeComponent();
            ViewModel = new(questionTypes);
            DataContext = ViewModel;
            HandleEvent();
        }

        public AddEditQuestionWindow(QuestionType[] questionTypes, Question question) {
            InitializeComponent();
            ViewModel = new(questionTypes, question);
            DataContext = ViewModel;
            HandleEvent();
        }

        private void HandleEvent() {
            ViewModel.PropertyChanged += (sender, e) => {
                if (e.PropertyName == "IsDialogResult") {
                    if (ViewModel.IsDialogResult)
                        DialogResult = true;
                    else
                        DialogResult = false;
                }
            };
        }
    }
}
