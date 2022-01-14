using Library;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace UserClient {
    class SendMessageServer {
        public static async Task SendAuthorizationMessage(TcpClient server, string login, string password) {
            MemoryStream stream = new MemoryStream();
            BinaryWriter writer = new BinaryWriter(stream);

            writer.Write(Message.Authorization);

            byte[] buffer = Encoding.UTF8.GetBytes(login);
            writer.Write(buffer.Length);
            writer.Write(buffer);

            buffer = Encoding.UTF8.GetBytes(password);
            writer.Write(buffer.Length);
            writer.Write(buffer);

            buffer = stream.ToArray();

            await server.GetStream().WriteAsync(buffer, 0, buffer.Length);
        }

        public static async Task SendSurveyListMessage(TcpClient server, Employee employee) {
            MemoryStream stream = new MemoryStream();
            BinaryWriter writer = new BinaryWriter(stream);

            writer.Write(Message.SurveyList);
            writer.Write(employee.Id);
            byte[] buffer = stream.ToArray();

            await server.GetStream().WriteAsync(buffer, 0, buffer.Length);
        }

        //public static async Task SendSelectedSurveyMessage(TcpClient server, Survey survey) {
        //    MemoryStream stream = new MemoryStream();
        //    BinaryWriter writer = new BinaryWriter(stream);

        //    writer.Write(Message.SelectedSurvey);
        //    writer.Write(survey.Id);
        //    byte[] buffer = stream.ToArray();

        //    await server.GetStream().WriteAsync(buffer, 0, buffer.Length);
        //}

        public static async Task SendEmployeeAnswerMessage(TcpClient server, EmployeeSurveyAnswer employeeAnswer) {
            MemoryStream stream = new MemoryStream();
            BinaryWriter writer = new BinaryWriter(stream);

            writer.Write(Message.EmployeeAnswers);
            string jsonString = JsonSerializer.Serialize(employeeAnswer, new() { IncludeFields = true });
            byte[] buffer = Encoding.UTF8.GetBytes(jsonString);
            writer.Write(buffer.Length);
            writer.Write(buffer);
            buffer = stream.ToArray();

            await server.GetStream().WriteAsync(buffer, 0, buffer.Length);
        }
    }
}
