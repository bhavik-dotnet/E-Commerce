using ECommerceAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace ECommerceAPI.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<ProductCategory> ProductCategories { get; set; }
        public DbSet<CartItem> CartItems { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<ProductRating> ProductRatings { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure relationships
            modelBuilder.Entity<ProductCategory>()
                .HasOne(pc => pc.Product)
                .WithMany(p => p.ProductCategories)
                .HasForeignKey(pc => pc.ProductId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ProductCategory>()
                .HasOne(pc => pc.Category)
                .WithMany(c => c.ProductCategories)
                .HasForeignKey(pc => pc.CategoryId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<CartItem>()
                .HasOne(ci => ci.User)
                .WithMany(u => u.CartItems)
                .HasForeignKey(ci => ci.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<CartItem>()
                .HasOne(ci => ci.Product)
                .WithMany(p => p.CartItems)
                .HasForeignKey(ci => ci.ProductId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Order>()
                .HasOne(o => o.User)
                .WithMany(u => u.Orders)
                .HasForeignKey(o => o.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<OrderItem>()
                .HasOne(oi => oi.Order)
                .WithMany(o => o.OrderItems)
                .HasForeignKey(oi => oi.OrderId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<OrderItem>()
                .HasOne(oi => oi.Product)
                .WithMany(p => p.OrderItems)
                .HasForeignKey(oi => oi.ProductId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ProductRating>()
                .HasOne(pr => pr.Product)
                .WithMany(p => p.ProductRatings)
                .HasForeignKey(pr => pr.ProductId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ProductRating>()
                .HasOne(pr => pr.User)
                .WithMany(u => u.ProductRatings)
                .HasForeignKey(pr => pr.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Seed initial data
            SeedData(modelBuilder);
        }

        private void SeedData(ModelBuilder modelBuilder)
        {
            // Seed Categories (static values only)
            modelBuilder.Entity<Category>().HasData(
                new Category { CategoryId = 1, CategoryName = "Men", CreatedDate = new DateTime(2025, 01, 01) },
                new Category { CategoryId = 2, CategoryName = "Women", CreatedDate = new DateTime(2025, 01, 01) },
                new Category { CategoryId = 3, CategoryName = "Kids", CreatedDate = new DateTime(2025, 01, 01) },
                new Category { CategoryId = 4, CategoryName = "Others", CreatedDate = new DateTime(2025, 01, 01) }
            );

            // Seed Users (static values only)
            modelBuilder.Entity<User>().HasData(
                //new User
                //{
                //    UserId = 1,
                //    Username = "customer",
                //    Email = "customer@gmail.com",
                //    PasswordHash = "$2a$11$8vJ5xqX5lYxK5yHqZ5qZ5e5qZ5qZ5qZ5qZ5qZ5qZ5qZ5qZ5qZ5qZ5",
                //    FullName = "Demo Customer",
                //    IsAdmin = false,
                //    CreatedDate = new DateTime(2025, 01, 01)
                //},
                new User
                {
                    UserId = 2,
                    Username = "admin",
                    Email = "admin@gmail.com",
                    PasswordHash = "$2a$11$8vJ5xqX5lYxK5yHqZ5qZ5e5qZ5qZ5qZ5qZ5qZ5qZ5qZ5qZ5qZ5qZ5",
                    FullName = "Admin User",
                    IsAdmin = true,
                    CreatedDate = new DateTime(2025, 01, 01)
                }
            );
        }


    }
}