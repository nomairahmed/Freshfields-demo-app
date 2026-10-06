using System;
using ExerciseApp.Model;

namespace ExerciseApp.Service.Rules
{
    public class AgeEligibilityRule(TimeProvider clock) : IQuoteRule
    {
        public const int MinimumAge = 17;
        public const int MaximumAge = 80;

        public string GetDeclineReason(QuoteRequest request)
        {
            var today = DateOnly.FromDateTime(clock.GetLocalNow().DateTime);
            var age = AgeOn(DateOnly.FromDateTime(request.DateOfBirth.Value), today);

            return age is < MinimumAge or > MaximumAge
                ? $"We can only provide quotes for drivers aged {MinimumAge} to {MaximumAge}."
                : null;
        }

        public static int AgeOn(DateOnly dateOfBirth, DateOnly date)
        {
            var age = date.Year - dateOfBirth.Year;
            if (dateOfBirth > date.AddYears(-age))
                age--;
            return age;
        }
    }
}
