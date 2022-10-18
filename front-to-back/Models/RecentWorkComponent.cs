using Microsoft.Build.Framework;
using System.ComponentModel.DataAnnotations;
using RequiredAttribute = System.ComponentModel.DataAnnotations.RequiredAttribute;

namespace front_to_back.Models
{
    public class RecentWorkComponent
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Ad mutleq doldurulmalidir"), MinLength(3, ErrorMessage = "Adin uzunlugu minimum 3 olmalidir")]
        public string Title { get; set; }

        [Required(ErrorMessage = "Text mutleq doldurulmalidir"), MinLength(10, ErrorMessage = "Text uzunlugu minimum 10 olmalidir")]
        public string Text { get; set; }

        [Required(ErrorMessage = "Mutleq doldurulmalidir")]
        public string FilePath { get; set; }
    }
}
