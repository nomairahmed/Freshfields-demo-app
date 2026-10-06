using System;
using ExerciseApp.Model;
using ExerciseApp.Service;
using ExerciseApp.Service.Pricing;
using ExerciseApp.Service.Rules;
using Microsoft.Extensions.Time.Testing;
using Xunit;

namespace ExerciseApp.Tests
{
    public class QuoteServiceTests
    {
        private static readonly QuoteRequest Request = new()
        {
            DateOfBirth = new DateTime(2000, 05, 01),
            InsuranceType = InsuranceType.FullyComprehensive,
            Make = "Ford",
            Model = "Focus"
        };

        [Fact]
        public void WhenDetailsAreProvided_AQuoteIsProduced()
        {
            var clock = new FakeTimeProvider(new DateTimeOffset(2026, 6, 15, 0, 0, 0, TimeSpan.Zero));
            var qs = new QuoteService(new PriceTable(), [new AgeEligibilityRule(clock)]);

            var result = qs.PerformQuote(Request);

            Assert.False(result.IsDeclined);
            Assert.Equal(200, result.Premium);
        }

        [Fact]
        public void WhenAllRulesPass_TheBasePriceIsReturned()
        {
            var qs = new QuoteService(new FixedPrice(123), [new FixedRule(null), new FixedRule(null)]);

            var result = qs.PerformQuote(Request);

            Assert.Equal(QuoteResult.Accepted(123), result);
        }

        [Fact]
        public void WhenARuleDeclines_TheQuoteIsDeclinedWithItsReason()
        {
            var qs = new QuoteService(new FixedPrice(123), [new FixedRule(null), new FixedRule("Not eligible")]);

            var result = qs.PerformQuote(Request);

            Assert.Equal(QuoteResult.Declined("Not eligible"), result);
        }

        [Fact]
        public void WhenARuleDeclines_NoPriceIsLookedUp()
        {
            var prices = new FixedPrice(123);
            var qs = new QuoteService(prices, [new FixedRule("Not eligible")]);

            qs.PerformQuote(Request);

            Assert.Equal(0, prices.Calls);
        }

        private class FixedPrice(decimal price) : IBasePriceProvider
        {
            public int Calls { get; private set; }

            public decimal GetBasePrice(InsuranceType insuranceType, string make, string model)
            {
                Calls++;
                return price;
            }
        }

        private class FixedRule(string declineReason) : IQuoteRule
        {
            public string GetDeclineReason(QuoteRequest request) => declineReason;
        }
    }
}
