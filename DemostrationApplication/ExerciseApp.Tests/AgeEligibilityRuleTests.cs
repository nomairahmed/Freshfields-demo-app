using System;
using ExerciseApp.Model;
using ExerciseApp.Service.Rules;
using Microsoft.Extensions.Time.Testing;
using Xunit;

namespace ExerciseApp.Tests
{
    public class AgeEligibilityRuleTests
    {
        private static readonly DateTimeOffset Today = new(2026, 6, 15, 12, 0, 0, TimeSpan.Zero);

        private readonly AgeEligibilityRule _rule = new(new FakeTimeProvider(Today));

        [Theory]
        [InlineData("2009-06-15")] // 17th birthday today
        [InlineData("1996-03-10")] // 30
        [InlineData("1946-06-15")] // 80th birthday today
        [InlineData("1945-06-16")] // 80, 81st birthday is tomorrow
        public void EligibleAges_AreAccepted(string dateOfBirth)
        {
            Assert.Null(_rule.GetDeclineReason(RequestFor(dateOfBirth)));
        }

        [Theory]
        [InlineData("2009-06-16")] // 16, 17th birthday is tomorrow
        [InlineData("2012-01-01")] // 14
        [InlineData("1945-06-15")] // 81st birthday today
        [InlineData("1930-01-01")] // 96
        [InlineData("2027-01-01")] // date of birth in the future
        public void IneligibleAges_AreDeclined(string dateOfBirth)
        {
            Assert.NotNull(_rule.GetDeclineReason(RequestFor(dateOfBirth)));
        }

        [Theory]
        [InlineData("2008-02-29", "2025-02-28", 16)]
        [InlineData("2008-02-29", "2025-03-01", 17)]
        [InlineData("2008-02-29", "2028-02-29", 20)]
        [InlineData("2000-12-31", "2026-01-01", 25)]
        public void AgeOn_HandlesBirthdaysAndLeapDays(string dateOfBirth, string date, int expectedAge)
        {
            Assert.Equal(expectedAge, AgeEligibilityRule.AgeOn(DateOnly.Parse(dateOfBirth), DateOnly.Parse(date)));
        }

        private static QuoteRequest RequestFor(string dateOfBirth) => new()
        {
            DateOfBirth = DateTime.Parse(dateOfBirth),
            InsuranceType = InsuranceType.FullyComprehensive,
            Make = "Ford",
            Model = "Focus"
        };
    }
}
