using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace QuotesWebAPI.Models
{
    public class Tag
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        public ICollection<TagAssignment> TagAssignments { get; set; }
    }
}
