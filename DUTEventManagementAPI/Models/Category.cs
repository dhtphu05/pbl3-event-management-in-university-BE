namespace DUTEventManagementAPI.Models
{
    public class Category
    {
        public string CategoryId { get; set; } = Guid.NewGuid().ToString();
        public string CategoryName { get; set; } = string.Empty;
    }
}
