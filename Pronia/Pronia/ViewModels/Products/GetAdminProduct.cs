using Pronia.Models;

namespace Pronia.ViewModels
{
    public class GetAdminProduct
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public string CategoryName { get; set; }
        public string Image { get; set; }
    }
}
