using System.ComponentModel.DataAnnotations;

namespace front_to_back.Models
{
    public class Category
    {
        public Category()
        {
              CategoryComponents = new List<CategoryComponent>();
        }
        public int Id { get; set; }

        [Required(ErrorMessage ="Ad mutleq doldurulmalidir"),MinLength(3,ErrorMessage ="Adin uzunlugu minimum 3 olmalidir")]
        public string Title { get; set; }
        public ICollection<CategoryComponent> CategoryComponents { get; set; }
    }
}
