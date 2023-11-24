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
            UdpClient udpClient = new UdpClient();
            IPEndPoint serverEndPoint = new IPEndPoint(IPAddress.Parse(Adress), Port);

            byte[] responseData = Encoding.UTF8.GetBytes(request);
            udpClient.Send(responseData, responseData.Length, serverEndPoint);

            IPEndPoint senderEndPoint = new IPEndPoint(IPAddress.Any, Port);
            byte[] receiveData = udpClient.Receive(ref senderEndPoint);
            response = Encoding.UTF8.GetString(receiveData);

            udpClient.Close();
        }

        private void LoadData()
        {
            SendRequest("1");
            foreach (string line in response.Split(new[] { '\n' }))
            {
                string str = line.ToString();
                string[] data = str.Split(',');
                if (data.Length == 5)
                {
                    Cinema_Model CinemaFilm = new Cinema_Model()
                    {
                        ID = int.Parse(data[0]),
                        Film = data[1],
                        DateTime = data[2],
                        Available_seats = bool.Parse(data[3]),
                        Total_seats = int.Parse(data[4]),
                    };
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
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < cinema.Count; i++)
            {
                string stStr = $"{cinema[i].Film}, {cinema[i].DateTime}, {cinema[i].Available_seats}, {cinema[i].Total_seats}";
                sb.AppendLine(stStr);
            }
            string requestData = sb.ToString();
            SendRequest("2" + "," + requestData);
        }
    }
}