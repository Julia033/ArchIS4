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
        //Объявление и инициализация логгера
        private static Logger logger = LogManager.GetCurrentClassLogger();
        private static DataController DC = new DataController();
        static async Task Main()
        {
            //Cоздание экземпляра класса UdpClient с параметром Port
            using UdpClient udpClient = new UdpClient(Port);
            //Запись в лог информации о запуске сервера и прослушивании порта
            logger.Info("Сервер запущен и ждет подключения...");

            while (true)
            {
                //Получение данных от клиента
                var result = await udpClient.ReceiveAsync();
                //Преобразование байтового массива в строку
                string request = Encoding.UTF8.GetString(result.Buffer);
                //Запись в лог информации о полученном запросе от клиента
                logger.Info($"Запрос получен от {result.RemoteEndPoint}: {request}");
                //Обработка запроса
                string response = ProcessRequest(request);
                //Преобразование строки в байтовый массив
                byte[] responseData = Encoding.UTF8.GetBytes(response);
                //Отправка ответа клиенту
                _ = udpClient.SendAsync(responseData, responseData.Length, result.RemoteEndPoint);
                //Запись в лог информации о отправленном ответе клиенту
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
            // Создание экземпляра класса
            StringBuilder sb = new StringBuilder();
            //добавляет строки с данными из базы данных
            for (int i = 0; i < DC.GetCinemas().Count; i++)
            {
                string sch = $"{DC.GetCinemas()[i].ID},{DC.GetCinemas()[i].Film},{DC.GetCinemas()[i].DateTime},{DC.GetCinemas()[i].Available_seats},{DC.GetCinemas()[i].Total_seats}";

                sb.AppendLine(sch);
            }
            return sb.ToString();
        }

        private static void AddRecord(string Film, DateTime DateTime, bool Available_seats, int Total_seats)
        {
            //Добавляет новую запись в базу данных
            DC.AddFilm(new Cinema { Film = Film, DateTime = DateTime, Available_seats = Available_seats, Total_seats = Total_seats });
        }
        private static void AddRecords(string request)
        {
            // Разбивает строку request на массив строк
            foreach (string line in request.Split(new char[] { '\n' }))
            {
                //Обрабатывает каждую строку из массива, разбивая ее на подстроки и добавляя новую запись в базу данных
                if (String.IsNullOrEmpty(line))
                { }
                else
                {
                    string str = line.ToString();
                    string[] data = str.Split(',');
                    AddRecord(data[0], DateTime.Parse(data[1]), bool.Parse(data[2]), int.Parse(data[3]));
                }
            }
        }
    }
}
