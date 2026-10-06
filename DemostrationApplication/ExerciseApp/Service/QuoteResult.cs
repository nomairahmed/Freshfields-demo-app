namespace ExerciseApp.Service
{
    public record QuoteResult(bool IsDeclined, decimal Premium, string DeclineReason)
    {
        public static QuoteResult Accepted(decimal premium) => new(false, premium, null);

        public static QuoteResult Declined(string reason) => new(true, 0, reason);
    }
}
