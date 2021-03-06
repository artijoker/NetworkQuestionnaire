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
    /// Логика взаимодействия для EmployeeAndAnswersWindow.xaml
    /// </summary>
    public partial class EmployeeAndAnswersWindow : Window {
        public EmployeeAndAnswersWindow(Survey survey) {
            InitializeComponent();
            EmployeeAndAnswersWindowViewModel viewModel = new(survey);
            DataContext = viewModel;
            viewModel.PropertyChanged += (sender, e) => {
                if (e.PropertyName == "IsReady")
                    DialogResult = true;
            };
        }
    }
}
