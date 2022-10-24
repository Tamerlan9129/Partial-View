using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace front_to_back.Models
{
    public class ContactBanner
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string Title { get; set; }
        [Required]
        public string Description { get; set; }
        public string? PhotoPath { get; set; }

        [NotMapped]
        public IFormFile Photo { get; set; }
    }
}
