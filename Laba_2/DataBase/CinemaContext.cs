using System.Data.Entity;

namespace Laba_2.DataBase
{
    class CinemaContext:DbContext
    {
        public CinemaContext():base("DbConnection")
        { }
        public DbSet<Cinema> Cinemas { get; set; }
    }
}
