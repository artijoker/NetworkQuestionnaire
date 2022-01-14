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
    /// Логика взаимодействия для AddEditSurveyWindow.xaml
    /// </summary>
    public partial class AddEditSurveyWindow : Window {

        public AddEditSurveyWindowViewModel ViewModel { get; }

        public AddEditSurveyWindow(QuestionType[] questionTypes) {
            InitializeComponent();
            ViewModel = new AddEditSurveyWindowViewModel(questionTypes);
            DataContext = ViewModel;
            ViewModel.PropertyChanged += (sender, e) => {
                if (e.PropertyName == "IsDialogResult") {
                    if (ViewModel.IsDialogResult)
                        DialogResult = true;
                    else
                        DialogResult = false;
                }
            };
        }

        public AddEditSurveyWindow(QuestionType[] questionTypes, Survey survey) {
            InitializeComponent();
            ViewModel = new AddEditSurveyWindowViewModel(questionTypes, survey);
            DataContext = ViewModel;
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
