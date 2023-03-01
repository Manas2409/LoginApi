using Microsoft.EntityFrameworkCore;
using WebApplication2.Models;
namespace WebApplication2.Data
{
    public class LoginContext:DbContext
    {
        public LoginContext(DbContextOptions<LoginContext> options) : base(options) { }
        public DbSet<UserDetails> UserDetails { get;set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Data Source=MANAS; database=BEproj; Integrated Security=True; Encrypt=False");
            base.OnConfiguring(optionsBuilder);
        }
    }
}
