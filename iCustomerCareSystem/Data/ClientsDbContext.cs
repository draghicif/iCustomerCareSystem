using iCustomerCareSystem.Core;
using iCustomerCareSystem.Models;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace iCustomerCareSystem.Data
{
    public class ClientsDbContext : DbContext
    {
        public DbSet<Client> Clients { get; set; }
        public DbSet<OperationType> OperationType { get; set; }
        public DbSet<ProductType> ProductType { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["MyServiceClients"].ConnectionString;
                optionsBuilder.UseMySQL(connectionString);
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Client>()
                .Property(c => c.ClientId)
                .ValueGeneratedOnAdd();
        }
    }
}
