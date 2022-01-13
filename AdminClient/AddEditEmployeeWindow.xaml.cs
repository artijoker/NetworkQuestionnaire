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
    /// Логика взаимодействия для AddEditEmployeeWindow.xaml
    /// </summary>
    public partial class AddEditEmployeeWindow : Window {
        public AddEditEmployeeWindowViewModel ViewModel { get; }

        public AddEditEmployeeWindow() {
            InitializeComponent();
            ViewModel = new AddEditEmployeeWindowViewModel();
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
        public AddEditEmployeeWindow(Employee employee) {
            InitializeComponent();
            ViewModel = new AddEditEmployeeWindowViewModel(employee);
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
