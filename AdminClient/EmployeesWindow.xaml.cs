using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
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
    /// Логика взаимодействия для EmployeesWindow.xaml
    /// </summary>
    public partial class EmployeesWindow : Window {
        public EmployeesWindow(TcpClient server) {
            InitializeComponent();
            EmployeesWindowViewModel viewModel = new(server);
            DataContext = viewModel;
        }
    }
}
