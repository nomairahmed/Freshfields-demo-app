using System.Threading.Tasks;
using ExerciseApp.Service;
using Microsoft.EntityFrameworkCore;

namespace ExerciseApp.Data
{
    public class EfQuoteRepository(QuoteDbContext db) : IQuoteRepository
    {
        public async Task<StoredQuote> AddAsync(StoredQuote quote)
        {
            do
            {
                quote.Reference = QuoteReference.Generate();
            }
            while (await db.Quotes.AnyAsync(q => q.Reference == quote.Reference));

            db.Quotes.Add(quote);
            await db.SaveChangesAsync();
            return quote;
        }

        public Task<StoredQuote> GetByReferenceAsync(string reference)
        {
            return db.Quotes.AsNoTracking().SingleOrDefaultAsync(q => q.Reference == reference);
        }
    }
}
