using API.Models;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
    public class UsersContext : DbContext
    {
        public UsersContext(DbContextOptions<UsersContext> opts) : base(opts)
        {
            Database.EnsureCreated();
        }
        public DbSet<User> Users { get; set; }
    }
}
