using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace WPF
{
    class Cinema_Model : INotifyPropertyChanged
    {
        public int ID { get; set; }
        public string Film
        {
            get
            {
                return film;
            }
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    throw new ArgumentException();
                }
                else
                {
                    film = value;
                    OnPropertyChanged("Film");
                }
            }
        }
        public DateTime DateTime
        {
            get
            {
                return datetime;
            }
            set
            {
                datetime = value;
                OnPropertyChanged("DateTime");
            }

        }
        public int Total_seats
        {
            get
            {
                return total_seats;
            }
            set
            {
                if (value < 0)
                {
                    total_seats = Math.Abs(value);
                    OnPropertyChanged("Total_seats");
                }
                else
                {
                    total_seats = value;
                    OnPropertyChanged("Total_seats");
                }


            }
        }
        public bool Available_seats
        {
            get
            {
                return available_seats;
            }
            set
            {
                available_seats = value;
                OnPropertyChanged("Available_seats");
            }
        }

        private string? film;
        private DateTime datetime;
        private int total_seats;
        private bool available_seats;

        public event PropertyChangedEventHandler? PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            //проверяет, что событие не равно null
            if (PropertyChanged != null)
                //вызывает его, передавая имя свойства
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }
    }
}