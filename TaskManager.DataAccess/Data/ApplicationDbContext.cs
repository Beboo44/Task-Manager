using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TaskManager.DataAccess.Models;

namespace TaskManager.DataAccess.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // DbSets for application entities
        public DbSet<UserTask> UserTasks { get; set; }
        public DbSet<Category> Categories { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Configure UserTask - User relationship (One-to-Many)
            // When a user is deleted, all their tasks are deleted
            builder.Entity<UserTask>()
                .HasOne(t => t.User)
                .WithMany(u => u.Tasks)
                .HasForeignKey(t => t.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Configure UserTask - Category relationship (Many-to-One)
            // Tasks cannot be deleted when a category is deleted (Restrict)
            builder.Entity<UserTask>()
                .HasOne(t => t.Category)
                .WithMany(c => c.Tasks)
                .HasForeignKey(t => t.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            // Configure Category - User relationship (One-to-Many)
            // When a user is deleted, all their categories are deleted
            builder.Entity<Category>()
                .HasOne(c => c.User)
                .WithMany()
                .HasForeignKey(c => c.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Add indexes for performance
            builder.Entity<UserTask>()
                .HasIndex(t => t.UserId);

            builder.Entity<UserTask>()
                .HasIndex(t => t.CategoryId);

            builder.Entity<UserTask>()
                .HasIndex(t => t.Deadline);

            builder.Entity<UserTask>()
                .HasIndex(t => t.Status);

            builder.Entity<Category>()
                .HasIndex(c => c.UserId);

            // No seed data - categories will be created per user upon registration
        }
    }
}
