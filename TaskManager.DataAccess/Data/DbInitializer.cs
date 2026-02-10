// Data Seeding and Initialization
// This contains logic for initializing default categories for new users
using TaskManager.DataAccess.Data;
using TaskManager.DataAccess.Models;
using TaskManager.DataAccess.Repositories;

namespace TaskManager.DataAccess.Data
{
    public static class DbInitializer
    {
        /// <summary>
        /// Default category templates that will be created for each new user
        /// </summary>
        public static readonly List<(string Name, string Description)> DefaultCategoryTemplates = new()
        {
            ("Work", "Work-related tasks"),
            ("Personal", "Personal tasks and errands"),
            ("Shopping", "Shopping and groceries"),
            ("Health", "Health and fitness tasks"),
            ("Education", "Learning and education"),
            ("Finance", "Financial tasks and bills")
        };

        /// <summary>
        /// Creates default categories for a new user
        /// </summary>
        public static async Task InitializeUserCategoriesAsync(ICategoryRepository categoryRepository, string userId)
        {
            await categoryRepository.CreateDefaultCategoriesForUserAsync(userId);
        }

        /// <summary>
        /// Ensures the database is created and migrated
        /// </summary>
        public static void Initialize(ApplicationDbContext context)
        {
            context.Database.EnsureCreated();
        }
    }
}
