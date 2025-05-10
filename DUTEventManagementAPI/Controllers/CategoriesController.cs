using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using DUTEventManagementAPI.Services;

namespace DUTEventManagementAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        private readonly ICategoryService _categoryService;
        public CategoriesController(ICategoryService categoryService)
        {
            _categoryService = categoryService
                ?? throw new ArgumentNullException(nameof(categoryService));
        }
        [HttpGet]
        public IActionResult GetAllCategories()
        {
            var categories = _categoryService.GetAllCategoriesAsync();
            if (categories == null || !categories.Any())
            {
                return NotFound("No categories found.");
            }
            return Ok(categories);
        }
        [HttpGet("{id}")]
        public IActionResult GetCategoryById(string id)
        {
            var category = _categoryService.GetCategoryById(id);
            return Ok(category);
        }
        [HttpPost]
        public IActionResult CreateCategory([FromBody] string categoryName)
        {
            var category = _categoryService.CreateCategory(categoryName);
            return CreatedAtAction(nameof(GetCategoryById), new { id = category.CategoryId }, category);
        }
        [HttpPut("{id}")]
        public IActionResult UpdateCategory(string id, [FromBody] string categoryName)
        {
            var updatedCategory = _categoryService.UpdateCategory(id, categoryName);
            return Ok(updatedCategory);
        }
        [HttpDelete("{id}")]
        public IActionResult DeleteCategory(string id)
        {
            var isDeleted = _categoryService.DeleteCategory(id);
            if (isDeleted)
            {
                return NoContent();
            }
            return NotFound("Event not found");
        }
    }
}
