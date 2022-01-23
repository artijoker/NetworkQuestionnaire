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
        public static async Task SendAdminConnectSuccessfulMessage(TcpClient client) {
            MemoryStream stream = new MemoryStream();
            BinaryWriter writer = new BinaryWriter(stream);

            writer.Write(Message.AdminConnectSuccessful);
            byte[] buffer = stream.ToArray();

            await client.GetStream().WriteAsync(buffer, 0, buffer.Length);
        }

        public static async Task SendAdminConnectFailedMessage(TcpClient client, string message) {
            MemoryStream stream = new MemoryStream();
            BinaryWriter writer = new BinaryWriter(stream);

            writer.Write(Message.AdminConnectFailed);
            byte[] buffer = Encoding.UTF8.GetBytes(message);
            writer.Write(buffer.Length);
            writer.Write(buffer);
            buffer = stream.ToArray();

            await client.GetStream().WriteAsync(buffer, 0, buffer.Length);
        }

        public static async Task SendAuthorizationFailedMessage(TcpClient client, string message) {
            MemoryStream stream = new MemoryStream();
            BinaryWriter writer = new BinaryWriter(stream);

            writer.Write(Message.AuthorizationFailed);
            byte[] buffer = Encoding.UTF8.GetBytes(message);
            writer.Write(buffer.Length);
            writer.Write(buffer);
            buffer = stream.ToArray();

            await client.GetStream().WriteAsync(buffer, 0, buffer.Length);
        }


        public static async Task SendAuthorizationSuccessfulMessage(TcpClient client, Employee employee) { 
            MemoryStream stream = new MemoryStream();
            BinaryWriter writer = new BinaryWriter(stream);

            writer.Write(Message.AuthorizationSuccessful);
            byte[] buffer = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(employee.ToDTO()));
            writer.Write(buffer.Length);
            writer.Write(buffer);
            buffer = stream.ToArray();

            await client.GetStream().WriteAsync(buffer, 0, buffer.Length);
        }

        public static async Task SendSurveyListMessage(TcpClient client, Survey[] surveys) {
            MemoryStream stream = new MemoryStream();
            BinaryWriter writer = new BinaryWriter(stream);


            writer.Write(Message.ListSurveysNotСompletedEmployee);

            byte[] buffer = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(surveys.Select(survey => survey.ToDTO()).ToArray()));
            writer.Write(buffer.Length);
            writer.Write(buffer);
            buffer = stream.ToArray();

            await client.GetStream().WriteAsync(buffer, 0, buffer.Length);
        }

        public static async Task SendAllSurveyAndQuestionTypeMessage(TcpClient client, Survey[] surveys, QuestionType[] questionTypes) {
            MemoryStream stream = new MemoryStream();
            BinaryWriter writer = new BinaryWriter(stream);


            writer.Write(Message.AllSurveyFromDBAndQuestionTypes);

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


            writer.Write(Message.EmployeesList);

            byte[] buffer = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(employees.Select(employee => employee.ToDTO()).ToArray()));
            writer.Write(buffer.Length);
            writer.Write(buffer);
            buffer = stream.ToArray();

            await client.GetStream().WriteAsync(buffer, 0, buffer.Length);
        }

        public static async Task SendAllAnswersEmployeeMessage(TcpClient client, Survey[] surveys) {
            MemoryStream stream = new MemoryStream();
            BinaryWriter writer = new BinaryWriter(stream);


            writer.Write(Message.SurveysСompletedEmployee);

            byte[] buffer = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(surveys.Select(survey => survey.ToDTO()).ToArray()));

            writer.Write(buffer.Length);
            writer.Write(buffer);
            buffer = stream.ToArray();

            await client.GetStream().WriteAsync(buffer, 0, buffer.Length);
        }
    }
}
