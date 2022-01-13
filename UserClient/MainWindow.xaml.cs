using Library;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
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

namespace UserClient {
    public partial class MainWindow : Window {
        public MainWindow(TcpClient server, Employee employee) {
            InitializeComponent();
            MainWindowViewModel ViewModel = new(server, employee);
            DataContext = ViewModel;
            ViewModel.PropertyChanged += (sender, e) => {
                if (e.PropertyName == "IsHide") 
                    Visibility = ViewModel.IsHide ? Visibility.Hidden : Visibility.Visible;
            };
        }
    }
}
