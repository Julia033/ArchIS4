using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Windows.Input;

namespace WPF
{
    class View_Model
    {
        public ICommand LoadDataCM { get; set; }
        public ICommand SaveDataCM { get; set; }
        public ObservableCollection<Cinema_Model> Cinemas
        {
            get { return cinema; }
            set
            {
                cinema = value;
            }
        }
        //Инициализация словаря
        Dictionary<int, Cinema_Model> CinemaDict = new Dictionary<int, Cinema_Model>();
        private ObservableCollection<Cinema_Model> cinema = new ObservableCollection<Cinema_Model>();
        private const int Port = 8001;
        private readonly string Adress = "127.0.0.1";
        private string? response;

        public View_Model()
        {
            LoadDataCM = new Command(LoadData);
            SaveDataCM = new Command(SaveData);
        }
        public void SendRequest(string request)
        {
            //Создание нового экземпляра класса
            UdpClient udpClient = new UdpClient();
            //Создание новой конечной точки сервера с указанным IP-адресом и портом
            IPEndPoint serverEndPoint = new IPEndPoint(IPAddress.Parse(Adress), Port);
            //Преобразование запроса в массив байтов
            byte[] responseData = Encoding.UTF8.GetBytes(request);
            udpClient.Send(responseData, responseData.Length, serverEndPoint);
            IPEndPoint senderEndPoint = new IPEndPoint(IPAddress.Any, Port);
            //Получение данных от сервера и сохранение их в массив байтов
            byte[] receiveData = udpClient.Receive(ref senderEndPoint);
            //Преобразование полученных данных из массива байтов в строку
            response = Encoding.UTF8.GetString(receiveData);
            //Закрытие соединения
            udpClient.Close();
        }

        private void LoadData()
        {
            SendRequest("1");
            //Разделяет полученные данные
            foreach (string line in response.Split(new[] { '\n' }))
            {
                //Преобразует каждую строку в тип данных string
                string str = line.ToString();
                //Разделяет строку на подстроки
                string[] data = str.Split(',');
                //Если длина массива data равна 5, то создается новый объект класса
                if (data.Length == 5)
                {
                    Cinema_Model CinemaFilm = new Cinema_Model()
                    {
                        ID = int.Parse(data[0]),
                        Film = data[1],
                        DateTime = DateTime.Parse(data[2]),
                        Available_seats = bool.Parse(data[3]),
                        Total_seats = int.Parse(data[4]),
                    };
                    //Если в словаре отсутствует элемент с ключом CinemaFilm.ID, то этот элемент добавляется в словарь и в список cinema.
                    if (!CinemaDict.ContainsKey(CinemaFilm.ID))
                    {
                        CinemaDict.Add(CinemaFilm.ID, CinemaFilm);
                        cinema.Add(CinemaFilm);
                    }
                }
            }
        }
        private void SaveData()
        {
            //Создает новый объект класса
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < cinema.Count; i++)
            {
                string Str = $"{cinema[i].Film}, {cinema[i].DateTime}, {cinema[i].Available_seats}, {cinema[i].Total_seats}";
                sb.AppendLine(Str);
            }
            //Преобразует содержимое объекта в строку
            string requestData = sb.ToString();
            SendRequest("2" + "," + requestData);
        }
    }
}