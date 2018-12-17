
using Microsoft.EntityFrameworkCore;

namespace LogReg.Models
{
    public class UserContext : DbContext
    {
        public UserContext(DbContextOptions<UserContext> options) : base(options) { }
        public DbSet<User> UsersTable {get;set;}
        
    }
}