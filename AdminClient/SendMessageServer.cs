using Library;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace AdminClient {
    class SendMessageServer {
        public static async Task SendEmployeeListMessage(TcpClient server) {
            MemoryStream stream = new MemoryStream();
            BinaryWriter writer = new BinaryWriter(stream);

            writer.Write(Message.EmployeeList);
            byte[] buffer = stream.ToArray();

            await server.GetStream().WriteAsync(buffer, 0, buffer.Length);
        }

        public static async Task SendAddNewEmployeeMessage(TcpClient server, Employee employee) {
            MemoryStream stream = new MemoryStream();
            BinaryWriter writer = new BinaryWriter(stream);

            writer.Write(Message.AddNewEmployee);

            byte[] buffer = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(employee.ToDTO()));
            writer.Write(buffer.Length);
            writer.Write(buffer);
            buffer = stream.ToArray();

            await server.GetStream().WriteAsync(buffer, 0, buffer.Length);
        }

        public static async Task SendEditEmployeeMessage(TcpClient server, Employee employee) {
            MemoryStream stream = new MemoryStream();
            BinaryWriter writer = new BinaryWriter(stream);

            writer.Write(Message.EditEmployee);

            byte[] buffer = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(employee.ToDTO()));
            writer.Write(buffer.Length);
            writer.Write(buffer);
            buffer = stream.ToArray();

            await server.GetStream().WriteAsync(buffer, 0, buffer.Length);
        }

        public static async Task SendRemoveEmployeeMessage(TcpClient server, Employee employee) {
            MemoryStream stream = new MemoryStream();
            BinaryWriter writer = new BinaryWriter(stream);

            writer.Write(Message.RemoveEmployee);

            byte[] buffer = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(employee.ToDTO()));
            writer.Write(buffer.Length);
            writer.Write(buffer);
            buffer = stream.ToArray();

            await server.GetStream().WriteAsync(buffer, 0, buffer.Length);
        }

        public static async Task SendAddNewSurveyMessage(TcpClient server, Survey survey) {
            MemoryStream stream = new MemoryStream();
            BinaryWriter writer = new BinaryWriter(stream);

            writer.Write(Message.AddNewSurvey);

            byte[] buffer = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(survey.ToDTO()));
            writer.Write(buffer.Length);
            writer.Write(buffer);
            buffer = stream.ToArray();

            await server.GetStream().WriteAsync(buffer, 0, buffer.Length);
        }

        public static async Task SendEditSurveyMessage(TcpClient server, Survey survey) {
            MemoryStream stream = new MemoryStream();
            BinaryWriter writer = new BinaryWriter(stream);

            writer.Write(Message.EditSurvey);

            byte[] buffer = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(survey.ToDTO()));
            writer.Write(buffer.Length);
            writer.Write(buffer);
            buffer = stream.ToArray();

            await server.GetStream().WriteAsync(buffer, 0, buffer.Length);
        }

        public static async Task SendRemoveSurveyMessage(TcpClient server, Survey survey) {
            MemoryStream stream = new MemoryStream();
            BinaryWriter writer = new BinaryWriter(stream);

            writer.Write(Message.RemoveSurvey);

            byte[] buffer = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(survey.ToDTO()));
            writer.Write(buffer.Length);
            writer.Write(buffer);
            buffer = stream.ToArray();

            await server.GetStream().WriteAsync(buffer, 0, buffer.Length);
        }
    }
}
