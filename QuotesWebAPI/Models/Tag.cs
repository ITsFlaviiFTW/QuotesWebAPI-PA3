using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace QuotesWebAPI.Models
{
    public class Tag
    {
        // This class is used to define the structure of the Tag object
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        public ICollection<TagAssignment> TagAssignments { get; set; }
    }
}
