using System.ComponentModel.DataAnnotations;

namespace QuotesWebAPI.Models
{
    // This class is used to define the structure of the QuoteDto object
    public class QuoteDto
    {
        [Required]
        public string Text { get; set; }
        public string Author { get; set; }
        public List<string> Tags { get; set; }
    }
}
