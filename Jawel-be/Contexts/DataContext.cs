using Jawel_be.Models;
using Microsoft.EntityFrameworkCore;

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

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
            optionsBuilder.UseSqlServer("Server=DESKTOP-QD7JEVF\\SQLEXPRESS;Database=jawel;Trusted_Connection=true;TrustServerCertificate=true;");
        }

        public virtual DbSet<Category> Categories { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Category>().HasData(
                new Category { id = 1, Name = "Category 1" },
                new Category { id = 2, Name = "Category 2" },
                new Category { id = 3, Name = "Category 3" }
            // Add more categories as needed
            );
        }
    }

}
