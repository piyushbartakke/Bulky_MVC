using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Bulky.Models
{
    public class Category
    {
        [Key] //only need this when the name is different than Id or CateoryId
        public int Id { get; set; }
        
        [Required]
        [MaxLength(30)]
        [DisplayName("Category Name")]
        public string Name { get; set; }

        [DisplayName("Display Order")]
        [Range(0, 100, ErrorMessage="Display Order must be between 1-100")]
        public int DisplayOrder { get; set; }
    }
}
