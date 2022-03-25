using API.Models;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
    public class TrophieContext : DbContext
    {
        public TrophieContext(DbContextOptions<TrophieContext> opts) : base(opts)
        {
            Database.EnsureCreated();
        }

        public DbSet<Trophie> Trophies { get; set; }
    }
}
