using Azure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuotesWebAPI.Data;
using QuotesWebAPI.Models;
using System.Linq;

[Route("api/quotes")]
[ApiController]
public class QuotesApiController : ControllerBase
{
    private readonly QuotesDbContext _context;

    public QuotesApiController(QuotesDbContext context)
    {
        _context = context;
    }

    // GET: quotes
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Quote>>> GetQuotes()
    {
        return await _context.Quotes
.Include(q => q.TagAssignments)
.ThenInclude(ta => ta.Tag)
.ToListAsync();


    }

    // POST: api/quotes
    [HttpPost]
    public async Task<ActionResult<Quote>> PostQuote(QuoteDto QuoteDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var quote = new Quote
        {
            Text = QuoteDto.Text,
            Author = QuoteDto.Author,
            Likes = 0 // assuming new quotes start with zero likes
        };

        foreach (var tagName in QuoteDto.Tags)
        {
            var tag = await _context.Tags.SingleOrDefaultAsync(t => t.Name == tagName)
                        ?? new Tag { Name = tagName };
            _context.Tags.Add(tag); // Add the tag if it's new
            quote.TagAssignments.Add(new TagAssignment { Tag = tag });
        }

        _context.Quotes.Add(quote);
        await _context.SaveChangesAsync();

        return CreatedAtAction("GetQuote", new { id = quote.Id }, quote);
    }



    // GET: quotes/5
    [HttpGet("{id}")]
    public async Task<ActionResult<Quote>> GetQuote(int id)
    {
        var quote = await _context.Quotes.FindAsync(id);

        if (quote == null)
        {
            return NotFound();
        }

        return quote;
    }
    // PUT: api/quotes/5
    [HttpPut("{id}")]
    public async Task<IActionResult> PutQuote(int id, [FromBody] QuoteDto quoteDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var quote = await _context.Quotes.FindAsync(id);
        if (quote == null)
        {
            return NotFound();
        }

        quote.Text = quoteDto.Text;
        quote.Author = quoteDto.Author; // Make sure to handle if Author can be null
                                        // Don't update Likes here, as they should be handled by a separate method

        try
        {
            await _context.SaveChangesAsync();
            return Ok(quote); // Return the updated quote object
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!QuoteExists(id))
            {
                return NotFound();
            }
            else
            {
                throw;
            }
        }

        return NoContent(); // Or return Ok(quote) if you want to send back the updated quote
    }


    // DELETE: api/quotes/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteQuote(int id)
    {
        var quote = await _context.Quotes.FindAsync(id);
        if (quote == null)
        {
            return NotFound();
        }

        _context.Quotes.Remove(quote);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    // Helper method to check if a quote exists
    private bool QuoteExists(int id) =>
        _context.Quotes.Any(e => e.Id == id);

    // POST: api/quotes/5/tags
    [HttpPost("{quoteId}/tags")]
    public async Task<ActionResult<Tag>> PostTag(int quoteId, Tag tag)
    {
        if (!QuoteExists(quoteId))
        {
            return NotFound();
        }

        // Add new tag if it doesn't exist
        var existingTag = await _context.Tags.FirstOrDefaultAsync(t => t.Name == tag.Name);
        if (existingTag == null)
        {
            _context.Tags.Add(tag);
            await _context.SaveChangesAsync();
            existingTag = tag;
        }

        // Associate tag with quote
        var tagAssignment = new TagAssignment { QuoteId = quoteId, TagId = existingTag.Id };
        _context.TagAssignments.Add(tagAssignment);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetTagsByQuoteId), new { id = existingTag.Id }, existingTag);
    }

    // GET: api/quotes/5/tags
    [HttpGet("{quoteId}/tags")]
    public async Task<ActionResult<IEnumerable<Tag>>> GetTagsByQuoteId(int quoteId)
    {
        if (!QuoteExists(quoteId))
        {
            return NotFound();
        }

        var tags = await _context.TagAssignments
            .Where(ta => ta.QuoteId == quoteId)
            .Select(ta => ta.Tag)
            .ToListAsync();

        return tags;
    }

    // POST: api/quotes/5/like
    [HttpPost("{id}/like")]
    public async Task<IActionResult> LikeQuote(int id)
    {
        var quote = await _context.Quotes.FindAsync(id);
        if (quote == null)
        {
            return NotFound();
        }

        quote.Likes++;
        await _context.SaveChangesAsync();

        return Ok(quote); // Return the updated quote with the like count
    }

    // GET: api/quotes/bytag/{tagName}
    [HttpGet("bytag/{tagName}")]
    public async Task<ActionResult<IEnumerable<Quote>>> GetQuotesByTagName(string tagName)
    {
        var tag = await _context.Tags.Include(t => t.TagAssignments)
                      .ThenInclude(ta => ta.Quote)
                      .SingleOrDefaultAsync(t => t.Name == tagName);

        if (tag == null)
        {
            return NotFound();
        }

        var quotes = tag.TagAssignments.Select(ta => ta.Quote);
        return Ok(quotes);
    }
}
