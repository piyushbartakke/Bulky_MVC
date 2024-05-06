using System.ComponentModel.DataAnnotations;

namespace BulkyWeb.Models
{
    public class Category
    {
        [Key] //only need this when the name is different than Id or CateoryId
        public int Id { get; set; }
        
        [Required]
        public string Name { get; set; }
        public int DisplayOrder { get; set; }
    }
}
