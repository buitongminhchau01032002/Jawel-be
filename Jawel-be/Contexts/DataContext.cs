using Jawel_be.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;

namespace Jawel_be.Contexts
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {

        }

        public DataContext()
        {

        }

        public virtual DbSet<Category> Categories { get; set; }
        public virtual DbSet<Product> Products { get; set; }
        public virtual DbSet<UserAccount> UserAccounts { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Product>()
                .HasOne(p => p.Category)
                .WithMany()
                .HasForeignKey(p => p.CategoryId)
                .IsRequired();

            modelBuilder.Entity<UserAccount>()
                .HasIndex(u => u.Username)
                .IsUnique();

            modelBuilder.Entity<Category>().HasData(
                new Category { Id = 1, Name = "Nhẫn" },
                new Category { Id = 2, Name = "Dây chuyền" },
                new Category { Id = 3, Name = "Vòng" },
                new Category { Id = 4, Name = "Bông tai" }
            );

            modelBuilder.Entity<Product>().HasData(
                new Product
                { 
                    Id = 1,
                    Name = "Nhẫn gì đó",
                    Description = "Mô tả",
                    Cost = 200000,
                    Price = 300000,
                    Quantity = 1,
                    CategoryId = 1
                }
            );

                
        }
    }

}
