using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace QuotesWebAPI.Models
{
    public class Quote
    {
        public int Id { get; set; }
        [Required]
        public string Text { get; set; }
        public string Author { get; set; }
        public int Likes { get; set; }
        public Quote()
        {
            TagAssignments = new HashSet<TagAssignment>();
        }
        public ICollection<TagAssignment> TagAssignments { get; set; }

    }
}
