using API.Models;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
    public class UsersContext : DbContext
    {
        public UsersContext(DbContextOptions<UsersContext> opts) : base(opts)
        {
            Database.EnsureCreated();
            foreach (var u in Users)
                u.InitializeTrophiesList();
        }
        public DbSet<User> Users { get; set; }
    }
}
