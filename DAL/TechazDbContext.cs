using backend.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace backend.DAL
{
    public class TechazDbContext : DbContext
    {
        private readonly IConfiguration _configuration;
        public TechazDbContext(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        // to-do: изменить на нужную
        public DbSet<Model> Models => Set<Model>();
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql(_configuration.GetConnectionString("DataBase")); 
            base.OnConfiguring(optionsBuilder);
        }
    }
}
