using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace API.Entity
{
    [Table("Categories")]
    public class Category
    {
        public int Id { get; set; }  // Primary Key
        public string Name { get; set; }
        public ICollection<Post> Posts { get; set; }  // Navigation property
    }
}