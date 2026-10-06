using System.Threading.Tasks;

namespace ExerciseApp.Data
{
    public interface IQuoteRepository
    {
        /// <summary>Saves the quote, assigning it a new unique reference.</summary>
        Task<StoredQuote> AddAsync(StoredQuote quote);

        /// <returns>The quote, or <c>null</c> if no quote has that reference.</returns>
        Task<StoredQuote> GetByReferenceAsync(string reference);
    }
}
