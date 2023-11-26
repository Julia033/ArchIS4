using Laba_2.DataBase;
using System.Collections.Generic;
using System;
using System.Linq;

namespace Laba_2
{
    class DataController
    {
        public int AddFilm(Cinema cinema)
        {
            using (CinemaContext db = new CinemaContext())
            {
                //Добавляет объект cinema в базу данных
                db.Cinemas.Add(cinema);
                db.SaveChanges();
                //получение ID первого кинотеатра в базе данных, отсортированного по ID
                int id = db.Cinemas.OrderBy(s => s.ID).First().ID;
                return id;
            }
        }

        public void DeleteFilm(int comand)
        {
            using (CinemaContext db = new CinemaContext())
            {

                var cinemas = db.Cinemas;
                Cinema cinema = db.Cinemas.Find(comand);
                if (cinema == null)
                {
                    throw new ArgumentException();
                }
                else
                {
                    db.Cinemas.Remove(cinema);
                    db.SaveChanges();
                }
            }
        }

        public Cinema GetCinema(int comand)
        {
            using (CinemaContext db = new CinemaContext())
            {
                //Возвращает первый объект из базы данных, который удовлетворяет условию
                return db.Cinemas.FirstOrDefault(p => p.ID == comand);
            }
        }

        public List<Cinema> GetCinemas()
        {
            using (CinemaContext db = new CinemaContext())
            {
                //Возвращает все кинотеатры из базы данных
                return db.Cinemas.ToList();
            }
        }
        public void DeleteDB()
        {
            using (CinemaContext db = new CinemaContext())
            {
                //Выполняет SQL-запрос для очистки таблицы Cinemas
                db.Database.ExecuteSqlCommand("TRUNCATE TABLE Cinemas");
                //Сбрасывает идентификатор таблицы Cinemas на 0
                db.Database.ExecuteSqlCommand("DBCC CHECKIDENT ('Cinemas', RESEED, 0)");
                db.SaveChanges();
            }
        }
    }
}
