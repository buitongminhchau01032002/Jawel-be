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

        public virtual DbSet<Category> Categories { get; set; }

        //protected override void OnModelCreating(ModelBuilder modelBuilder)
        //{
        //    base.OnModelCreating(modelBuilder);

        //    modelBuilder.Entity<Category>().HasData(
        //        new Category { Id = 1, Name = "Nhẫn" },
        //        new Category { Id = 2, Name = "Dây chuyền" },
        //        new Category { Id = 3, Name = "Vòng" },
        //        new Category { Id = 4, Name = "Bông tai" }
        //    );
        //}
    }

}
