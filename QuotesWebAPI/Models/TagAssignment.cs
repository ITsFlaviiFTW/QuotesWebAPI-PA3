namespace QuotesWebAPI.Models
{
    public class TagAssignment
    {
        // This class is used to define the structure of the TagAssignment object
        public int QuoteId { get; set; }
        public Quote Quote { get; set; }
        public int TagId { get; set; }
        public Tag Tag { get; set; }
    }
}