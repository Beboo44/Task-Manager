using TaskManager.DataAccess.Models;

namespace TaskManager.DataAccess.Repositories
{
    public interface ICategoryRepository : IRepository<Category>
    {
        Task<IEnumerable<Category>> GetUserCategoriesAsync(string userId);
        Task<Category?> GetCategoryWithTasksAsync(int id);
        Task CreateDefaultCategoriesForUserAsync(string userId);
    }
}
