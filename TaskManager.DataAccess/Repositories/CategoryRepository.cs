using Microsoft.EntityFrameworkCore;
using TaskManager.DataAccess.Data;
using TaskManager.DataAccess.Models;

namespace TaskManager.DataAccess.Repositories
{
    public class CategoryRepository : Repository<Category>, ICategoryRepository
    {
        public CategoryRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Category>> GetUserCategoriesAsync(string userId)
        {
            return await _dbSet
                .Include(c => c.Tasks)
                .Where(c => c.UserId == userId)
                .OrderBy(c => c.Name)
                .ToListAsync();
        }

        public async Task<Category?> GetCategoryWithTasksAsync(int id)
        {
            return await _dbSet
                .Include(c => c.Tasks)
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task CreateDefaultCategoriesForUserAsync(string userId)
        {
            var defaultCategories = new List<Category>
            {
                new Category { Name = "Work", Description = "Work-related tasks", UserId = userId, CreatedAt = DateTime.UtcNow },
                new Category { Name = "Personal", Description = "Personal tasks and errands", UserId = userId, CreatedAt = DateTime.UtcNow },
                new Category { Name = "Shopping", Description = "Shopping and groceries", UserId = userId, CreatedAt = DateTime.UtcNow },
                new Category { Name = "Health", Description = "Health and fitness tasks", UserId = userId, CreatedAt = DateTime.UtcNow },
                new Category { Name = "Education", Description = "Learning and education", UserId = userId, CreatedAt = DateTime.UtcNow },
                new Category { Name = "Finance", Description = "Financial tasks and bills", UserId = userId, CreatedAt = DateTime.UtcNow }
            };

            await _dbSet.AddRangeAsync(defaultCategories);
            await _context.SaveChangesAsync();
        }
    }
}
