using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Pronia.ViewModels
{
    public class CreateSlideVM
    {
        [MaxLength(50, ErrorMessage = "The input cannot exceed 50 characters.")]
        public string Title { get; set; }
        [MaxLength(50, ErrorMessage = "The input cannot exceed 50 characters.")]
        public string SubTitle { get; set; }
        public string Description { get; set; }
        public int Order { get; set; }
        public IFormFile Photo { get; set; }

    }
}
