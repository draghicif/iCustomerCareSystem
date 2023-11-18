using iCustomerCareSystem.Core;
using iCustomerCareSystem.Models;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace iCustomerCareSystem.Data
{
    public class ClientsDbContext : DbContext
    {
        public DbSet<ClientProducts> ClientProducts { get; set; }
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
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<ClientProducts>()
                .HasKey(cp => cp.ClientProductId);

            modelBuilder.Entity<ClientProducts>()
                .Property(cp => cp.ClientProductId)
                .ValueGeneratedOnAdd();

            modelBuilder.Entity<ClientProducts>()
                .HasOne(cp => cp.Client)
                .WithMany(c => c.ClientProducts)
                .HasForeignKey(cp => cp.ClientId);

            modelBuilder.Entity<ClientProducts>()
                .HasOne(cp => cp.OperationType)
                .WithMany(ot => ot.ClientProducts)
                .HasForeignKey(cp => cp.OperationTypeId);

            modelBuilder.Entity<ClientProducts>()
                .HasOne(cp => cp.ProductType)
                .WithMany(pt => pt.ClientProducts)
                .HasForeignKey(cp => cp.ProductTypeId);
        }
    }
}
