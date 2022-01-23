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

namespace UserClient {
    /// <summary>
    /// Логика взаимодействия для SurveyWindow.xaml
    /// </summary>
    public partial class SurveyWindow : Window {
        public SurveyWindowViewModel ViewModel { get; }
        public SurveyWindow(Survey survey) {
            InitializeComponent();
            ViewModel = new SurveyWindowViewModel(survey);
            DataContext = ViewModel;
            ViewModel.PropertyChanged += (sender, e) => {
                if (e.PropertyName == "IsReady")
                    DialogResult = true;

            };
        }
    }
}
