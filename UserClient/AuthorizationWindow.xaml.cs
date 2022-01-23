using System;
using System.Collections.Generic;
using System.ComponentModel;
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

    public partial class AuthorizationWindow : Window {
        AuthorizationWindowViewModel ViewModel;
        public AuthorizationWindow() {
            InitializeComponent();
            ViewModel = new();
            DataContext = ViewModel;
            ViewModel.PropertyChanged += (sender, e) => {
                if (e.PropertyName == "IsHide") 
                    Visibility = Visibility.Hidden;
                if (e.PropertyName == "IsClose")
                    Close();
            };
            PasswordBox.PasswordChanged += (sender, e) => ViewModel.Password = PasswordBox.Password;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e) {
            ViewModel.WindowLoaded();
        }
    }
}
