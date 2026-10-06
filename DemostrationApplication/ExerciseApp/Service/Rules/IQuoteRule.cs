using ExerciseApp.Model;

namespace ExerciseApp.Service.Rules
{
    public interface IQuoteRule
    {
        /// <returns>The reason the quote is declined, or <c>null</c> if the rule is satisfied.</returns>
        string GetDeclineReason(QuoteRequest request);
    }
}
