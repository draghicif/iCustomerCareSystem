using iCustomerCareSystem.Models;
using Microsoft.EntityFrameworkCore;

namespace iCustomerCareSystem.Data
{
    public class ClientsDbContext : DbContext
    {
        public DbSet<ClientProducts> ClientProducts { get; set; }
        public DbSet<Client> Clients { get; set; }
        public DbSet<ProductType> ProductType { get; set; }
        public DbSet<ProductStatus> ProductStatus { get; set; }

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

            modelBuilder.Entity<Client>()
                .HasKey(cp => cp.ClientId);

            modelBuilder.Entity<Client>()
                .Property(cp => cp.ClientId)
                .ValueGeneratedOnAdd();

            modelBuilder.Entity<ClientProducts>()
                .Property(cp => cp.ClientProductId)
                .ValueGeneratedOnAdd();

            modelBuilder.Entity<ClientProducts>()
                .HasOne(cp => cp.Client)
                .WithMany(c => c.ClientProducts)
                .HasForeignKey(cp => cp.ClientId);

            modelBuilder.Entity<ClientProducts>()
                .HasOne(cp => cp.ProductType)
                .WithMany(pt => pt.ClientProducts)
                .HasForeignKey(cp => cp.ProductTypeId);

            modelBuilder.Entity<ClientProducts>()
                .HasOne(cp => cp.ProductStatus)
                .WithMany(pt => pt.ClientProducts)
                .HasForeignKey(cp => cp.ProductStatusId);

            modelBuilder.Entity<ClientProducts>()
                .HasOne(cp => cp.Client)
                .WithMany(ot => ot.ClientProducts)
                .HasForeignKey(cp => cp.ClientId);
        }
    }
}
