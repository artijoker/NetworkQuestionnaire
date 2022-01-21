using Library;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Server {
    class SendMessageClient {
        public static async Task SendAuthorizationMessage(TcpClient client) {
            MemoryStream stream = new MemoryStream();
            BinaryWriter writer = new BinaryWriter(stream);

            writer.Write(Message.Authorization);
            byte[] buffer = Encoding.UTF8.GetBytes("Ошибка! Неверный логин или пароль");
            writer.Write(buffer.Length);
            writer.Write(buffer);
            buffer = stream.ToArray();

            await client.GetStream().WriteAsync(buffer, 0, buffer.Length);
        }


        public static async Task SendСonnectionMessage(TcpClient client, Employee employee) { 
            MemoryStream stream = new MemoryStream();
            BinaryWriter writer = new BinaryWriter(stream);

            writer.Write(Message.Сonnection);
            byte[] buffer = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(employee.ToDTO()));
            writer.Write(buffer.Length);
            writer.Write(buffer);
            buffer = stream.ToArray();

            await client.GetStream().WriteAsync(buffer, 0, buffer.Length);
        }

        public static async Task SendSurveyListMessage(TcpClient client, Survey[] surveys) {
            MemoryStream stream = new MemoryStream();
            BinaryWriter writer = new BinaryWriter(stream);


            writer.Write(Message.SurveyList);

            byte[] buffer = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(surveys.Select(survey => survey.ToDTO()).ToArray()));
            writer.Write(buffer.Length);
            writer.Write(buffer);
            buffer = stream.ToArray();

            await client.GetStream().WriteAsync(buffer, 0, buffer.Length);
        }

        public static async Task SendAllSurveyAndQuestionTypeMessage(TcpClient client, Survey[] surveys, QuestionType[] questionTypes) {
            MemoryStream stream = new MemoryStream();
            BinaryWriter writer = new BinaryWriter(stream);


            writer.Write(Message.AllSurveyAndQuestionTypes);

            byte[] buffer = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(surveys.Select(survey => survey.ToDTO()).ToArray()));
            writer.Write(buffer.Length);
            writer.Write(buffer);
            buffer = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(questionTypes.Select(questionType => questionType.ToDTO()).ToArray()));
            writer.Write(buffer.Length);
            writer.Write(buffer);
            buffer = stream.ToArray();

            await client.GetStream().WriteAsync(buffer, 0, buffer.Length);
        }


        public static async Task SendDataSaveSuccessMessage(TcpClient client, string message) {
            MemoryStream stream = new MemoryStream();
            BinaryWriter writer = new BinaryWriter(stream);

            writer.Write(Message.DataSaveSuccess);
            byte[] buffer = Encoding.UTF8.GetBytes(message);
            writer.Write(buffer.Length);
            writer.Write(buffer);
            buffer = stream.ToArray();

            await client.GetStream().WriteAsync(buffer, 0, buffer.Length);
        }

        public static async Task SendEmployeeListMessage(TcpClient client, Employee[] employees) {
            MemoryStream stream = new MemoryStream();
            BinaryWriter writer = new BinaryWriter(stream);


            writer.Write(Message.EmployeeList);

            byte[] buffer = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(employees.Select(employee => employee.ToDTO()).ToArray()));
            writer.Write(buffer.Length);
            writer.Write(buffer);
            buffer = stream.ToArray();

            await client.GetStream().WriteAsync(buffer, 0, buffer.Length);
        }

        public static async Task SendAllAnswersEmployeeMessage(TcpClient client, Survey[] surveys) {
            MemoryStream stream = new MemoryStream();
            BinaryWriter writer = new BinaryWriter(stream);


            writer.Write(Message.AllAnswersEmployee);

            byte[] buffer = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(surveys.Select(survey => survey.ToDTO()).ToArray()));
            writer.Write(buffer.Length);
            writer.Write(buffer);
            buffer = stream.ToArray();

            await client.GetStream().WriteAsync(buffer, 0, buffer.Length);
        }
    }
}
