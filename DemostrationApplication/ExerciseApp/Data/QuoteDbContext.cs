using ExerciseApp.Service;
using Microsoft.EntityFrameworkCore;

namespace ExerciseApp.Data
{
    public class QuoteDbContext(DbContextOptions<QuoteDbContext> options) : DbContext(options)
    {
        public DbSet<StoredQuote> Quotes => Set<StoredQuote>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            var quote = modelBuilder.Entity<StoredQuote>();

            quote.HasIndex(q => q.Reference).IsUnique();
            quote.Property(q => q.Reference).IsRequired().HasMaxLength(QuoteReference.Length);
            quote.Property(q => q.Make).IsRequired().HasMaxLength(10);
            quote.Property(q => q.Model).IsRequired().HasMaxLength(10);
            quote.Property(q => q.InsuranceType).HasConversion<string>().HasMaxLength(30);
        }
    }
}
