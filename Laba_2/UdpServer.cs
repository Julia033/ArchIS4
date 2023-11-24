using NLog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;


namespace Laba_2
{
    public class Server
    {
        private const int Port = 8001;
        private static Logger logger = LogManager.GetCurrentClassLogger();
        private static DataController DC = new DataController();
        static async Task Main()
        {
            Console.BackgroundColor = ConsoleColor.DarkGray;
            Console.Clear();
            using UdpClient udpClient = new UdpClient(Port);

            logger.Info("Сервер запущен и ждет подключения...");

            while (true)
            {
                var result = await udpClient.ReceiveAsync();
                string request = Encoding.UTF8.GetString(result.Buffer);

                logger.Info($"Запрос получен от {result.RemoteEndPoint}: {request}");

                string response = ProcessRequest(request);
                byte[] responseData = Encoding.UTF8.GetBytes(response);

                _ = udpClient.SendAsync(responseData, responseData.Length, result.RemoteEndPoint);

                logger.Info($"Запрос отправлен: {result.RemoteEndPoint}: {response}");
            }
        }

        private static string ProcessRequest(string request)
        {
            string[] data = request.Split(',');
            string str = data[0];
            string result = request.Replace(str + ",", string.Empty);

            switch (str)
            {
                case "1":
                    return GetAllRecords();
                case "2":
                    DC.DeleteDB();
                    AddRecords(result);
                    return "Изменения сохранены";
                default:
                    return "Что-то пошло не так";
            }
        }
        private static string GetAllRecords()
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < DC.GetCinemas().Count; i++)
            {
                string sch = $"{DC.GetCinemas()[i].ID},{DC.GetCinemas()[i].Film},{DC.GetCinemas()[i].DateTime},{DC.GetCinemas()[i].Available_seats},{DC.GetCinemas()[i].Total_seats}";

                sb.AppendLine(sch);
            }
            return sb.ToString();
        }

        private static void AddRecord(string Film, string DateTime, bool Available_seats, int Total_seats)
        {
            DC.AddFilm(new Cinema { Film = Film, DateTime = DateTime, Available_seats = Available_seats, Total_seats = Total_seats });
        }
        private static void AddRecords(string request)
        {
            foreach (string line in request.Split(new char[] { '\n' }))
            {
                if (String.IsNullOrEmpty(line))
                { }
                else
                {
                    string str = line.ToString();
                    string[] data = str.Split(',');
                    AddRecord(data[0], data[1], bool.Parse(data[2]), int.Parse(data[3]));
                }
            }
        }
    }
}
