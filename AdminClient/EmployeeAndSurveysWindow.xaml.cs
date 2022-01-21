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
    /// Логика взаимодействия для EmployeeAndSurveysWindow.xaml
    /// </summary>
    public partial class EmployeeAndSurveysWindow : Window {
        public EmployeeAndSurveysWindow(Survey[] surveys) {
            InitializeComponent();
            EmployeeAndSurveysWindowViewModel viewModel = new EmployeeAndSurveysWindowViewModel(surveys);
            DataContext = viewModel;
            viewModel.PropertyChanged += (sender, e) => {
                if (e.PropertyName == "IsDialogResult")
                    if (viewModel.IsDialogResult)
                        DialogResult = true;
                    else
                        DialogResult = false;
            };
        }
    }
}
