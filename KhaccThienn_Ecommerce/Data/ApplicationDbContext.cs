using KhaccThienn_Ecommerce.Enums;
using KhaccThienn_Ecommerce.Models.DataModels;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Reflection.Emit;

namespace KhaccThienn_Ecommerce.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
           
            builder.Entity<Role>().HasData(
                new Role{ Id=0, Name= "Admin", Created_Date = DateTime.Now, Descriptions = "Admin Of Application" },
                new Role { Id = 1, Name = "User", Created_Date = DateTime.Now, Descriptions = "User Of Application" } 
                );

            builder.Entity<Order>()
                .Property(x => x.Status)
                .HasDefaultValue(OrderStatuses.PROCESSING);
        }


        public DbSet<Category> Categories { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderDetail> OrderDetails { get; set; }
        public DbSet<Cart> Carts { get; set; } 
        public DbSet<Contact> Contacts { get; set; }
    }
}
