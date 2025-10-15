using System;
using Microsoft.EntityFrameworkCore;

namespace aspnetapp
{
    public partial class AppDbContext : DbContext
    {
        public AppDbContext()
        {
        }

        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        // 两个表
        public DbSet<Employee> Employees { get; set; } = null!;
        public DbSet<Account> Accounts { get; set; } = null!;
        public DbSet<Order> Orders { get; set; } = null!;
        public DbSet<KYS> KYS { get; set; } = null!;
        public DbSet<MYS> MYS { get; set; } = null!;
        public DbSet<DZXH> DZXH { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                var username = Environment.GetEnvironmentVariable("MYSQL_USERNAME");
                var password = Environment.GetEnvironmentVariable("MYSQL_PASSWORD");
                var addressParts = Environment.GetEnvironmentVariable("MYSQL_ADDRESS")?.Split(':');
                var host = addressParts?[0];
                var port = addressParts?[1];
                var connstr = $"server={host};port={port};user={username};password={password};database=aspnet_demo";
                optionsBuilder.UseMySql(connstr, Microsoft.EntityFrameworkCore.ServerVersion.Parse("5.7.18-mysql"));
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.UseCollation("utf8_general_ci")
                .HasCharSet("utf8");

            // 映射到数据库表
            modelBuilder.Entity<Employee>().ToTable("Employees");
            modelBuilder.Entity<Account>().ToTable("Accounts");
            modelBuilder.Entity<Order>().ToTable("Orders");
            modelBuilder.Entity<KYS>().ToTable("KYS");
            modelBuilder.Entity<MYS>().ToTable("MYS");
            modelBuilder.Entity<DZXH>().ToTable("DZXH").HasNoKey();

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
