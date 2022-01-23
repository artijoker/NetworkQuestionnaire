using Library;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Server {
    class ConnectedEmployee {

        public Employee Employee { get; }
        public TcpClient Client { get; }

        public ConnectedEmployee(Employee employee, TcpClient client) {
            Employee = employee;
            Client = client;
        }

    }
}
