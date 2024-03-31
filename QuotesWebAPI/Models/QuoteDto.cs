using System.ComponentModel.DataAnnotations;

namespace QuotesWebAPI.Models
{
    public class QuoteDto
    {
        [Required]
        public string Text { get; set; }
        public string Author { get; set; }
        public List<string> Tags { get; set; }
    }
}
