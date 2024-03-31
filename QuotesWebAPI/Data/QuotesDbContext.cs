using QuotesWebAPI.Models;
using System.Collections.Generic;
using System.Reflection.Emit;
using Microsoft.EntityFrameworkCore;


namespace QuotesWebAPI.Data
{
    public class QuotesDbContext : DbContext
    {
        public QuotesDbContext(DbContextOptions<QuotesDbContext> options) : base(options) { }

        public DbSet<Quote> Quotes { get; set; }
        public DbSet<Tag> Tags { get; set; }
        public DbSet<TagAssignment> TagAssignments { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<TagAssignment>()
                .HasKey(t => new { t.QuoteId, t.TagId });

            modelBuilder.Entity<TagAssignment>()
                .HasOne(pt => pt.Quote)
                .WithMany(p => p.TagAssignments)
                .HasForeignKey(pt => pt.QuoteId);

            modelBuilder.Entity<TagAssignment>()
                .HasOne(pt => pt.Tag)
                .WithMany(t => t.TagAssignments)
                .HasForeignKey(pt => pt.TagId);
        }
    }

}
