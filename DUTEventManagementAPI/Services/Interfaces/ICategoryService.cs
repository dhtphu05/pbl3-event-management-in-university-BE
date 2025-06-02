using DUTEventManagementAPI.Models;

namespace DUTEventManagementAPI.Services.Interfaces
{
    public interface ICategoryService
    {
        Category CreateCategory(string categoryName);
        bool DeleteCategory(string id);
        List<Category> GetAllCategoriesAsync();
        Category GetCategoryById(string id);
        Category UpdateCategory(string id, string categoryName);
    }
}