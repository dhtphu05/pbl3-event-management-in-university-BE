using DUTEventManagementAPI.Data;
using DUTEventManagementAPI.Models;

namespace DUTEventManagementAPI.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly AppDbContext _context;
        public CategoryService(AppDbContext context)
        {
            _context = context;
        }
        public List<Category> GetAllCategoriesAsync()
        {
            return _context.Categories.ToList();
        }

        public Category GetCategoryById(string id)
        {
            var categoryResult = _context.Categories.FirstOrDefault(c => c.CategoryId == id);
            if (categoryResult == null)
            {
                throw new Exception("Category not found");
            }
            return categoryResult;
        }

        public Category CreateCategory(string categoryName)
        {
            var category = new Category
            {
                CategoryName = categoryName
            };
            _context.Categories.Add(category);
            _context.SaveChanges();
            return category;
        }

        public Category UpdateCategory(string id, string categoryName)
        {
            var category = _context.Categories.FirstOrDefault(c => c.CategoryId == id);
            if (category == null)
            {
                throw new Exception("Category not found");
            }
            category.CategoryName = categoryName;
            _context.Categories.Update(category);
            _context.SaveChanges();
            return category;
        }

        public bool DeleteCategory(string id)
        {
            var category = _context.Categories.FirstOrDefault(c => c.CategoryId == id);
            if (category == null)
                return false;
            _context.Categories.Remove(category);
            _context.SaveChanges();
            return true;
        }
    }
}
