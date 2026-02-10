using AutoMapper;
using TaskManager.Business.DTOs;
using TaskManager.DataAccess.Models;
using TaskManager.DataAccess.Repositories;

namespace TaskManager.Business.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly IMapper _mapper;

        public CategoryService(ICategoryRepository categoryRepository, IMapper mapper)
        {
            _categoryRepository = categoryRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<CategoryDto>> GetAllCategoriesAsync(string userId)
        {
            var categories = await _categoryRepository.GetUserCategoriesAsync(userId);
            return _mapper.Map<IEnumerable<CategoryDto>>(categories);
        }

        public async Task<CategoryDto?> GetCategoryByIdAsync(int id, string userId)
        {
            var category = await _categoryRepository.GetCategoryWithTasksAsync(id);
            if (category == null || category.UserId != userId)
                return null;

            return _mapper.Map<CategoryDto>(category);
        }

        public async Task<CategoryDto> CreateCategoryAsync(CategoryDto categoryDto)
        {
            var category = _mapper.Map<Category>(categoryDto);
            category.CreatedAt = DateTime.UtcNow;

            await _categoryRepository.AddAsync(category);
            await _categoryRepository.SaveAsync();

            return _mapper.Map<CategoryDto>(category);
        }

        public async Task<bool> UpdateCategoryAsync(CategoryDto categoryDto)
        {
            var existingCategory = await _categoryRepository.GetByIdAsync(categoryDto.Id);
            if (existingCategory == null || existingCategory.UserId != categoryDto.UserId)
                return false;

            existingCategory.Name = categoryDto.Name;
            existingCategory.Description = categoryDto.Description;

            await _categoryRepository.UpdateAsync(existingCategory);
            await _categoryRepository.SaveAsync();

            return true;
        }

        public async Task<bool> DeleteCategoryAsync(int id, string userId)
        {
            var category = await _categoryRepository.GetCategoryWithTasksAsync(id);
            if (category == null || category.UserId != userId)
                return false;

            // Check if category has tasks
            if (category.Tasks.Any())
                return false;

            await _categoryRepository.DeleteAsync(category);
            await _categoryRepository.SaveAsync();

            return true;
        }

        public async Task InitializeDefaultCategoriesAsync(string userId)
        {
            await _categoryRepository.CreateDefaultCategoriesForUserAsync(userId);
        }

        public async Task<bool> CanDeleteCategoryAsync(int id, string userId)
        {
            var category = await _categoryRepository.GetCategoryWithTasksAsync(id);
            if (category == null || category.UserId != userId)
                return false;

            return !category.Tasks.Any();
        }
    }
}
