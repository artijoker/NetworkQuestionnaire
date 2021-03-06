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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace AdminClient {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {
        private MainWindowViewModel ViewModel;
        public MainWindow() {
            InitializeComponent();
            ViewModel = new();
            DataContext = ViewModel;
            ViewModel.PropertyChanged += (sender, e) => {
                if (e.PropertyName == "IsHide")
                    Visibility = ViewModel.IsHide ? Visibility.Hidden : Visibility.Visible;
                if (e.PropertyName == "IsClose")
                    Close();
            };
        }

        private void Window_Loaded(object sender, RoutedEventArgs e) {
            ViewModel.WindowLoaded();
        }
    }
}
