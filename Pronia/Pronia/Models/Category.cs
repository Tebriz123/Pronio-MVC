namespace Pronia.Models
{
    public class Category:BaseEntity
    {
        public string Name { get; set; }
        //relationl
        public List<Product> Products { get; set; }
    }
}
 