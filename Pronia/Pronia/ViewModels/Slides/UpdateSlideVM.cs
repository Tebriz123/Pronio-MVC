using System.ComponentModel.DataAnnotations;

namespace Pronia.ViewModels
{
    public class UpdateSlideVM
    {
        [MaxLength(50, ErrorMessage = "The input cannot exceed 50 characters.")]
        public string Title { get; set; }
        public string SubTitle { get; set; }
        public string Description { get; set; }
        public string Image { get; set; }
        public int Order { get; set; }
        public IFormFile? Photo { get; set; }

    }
}
