using System;
using ExerciseApp.Model;
using ExerciseApp.Service.Pricing;
using Xunit;

namespace ExerciseApp.Tests
{
    public class PriceTableTests
    {
        private readonly PriceTable _priceTable = new();

        [Theory]
        [InlineData(InsuranceType.FullyComprehensive, "Ford", "Focus", 200)]
        [InlineData(InsuranceType.FullyComprehensive, "BMW", "X5", 500)]
        [InlineData(InsuranceType.FullyComprehensive, "BMW", "3 Series", 400)]
        [InlineData(InsuranceType.FullyComprehensive, "Audi", "A3", 300)]
        [InlineData(InsuranceType.ThirdPartyFireAndTheft, "Ford", "Fiesta", 180)]
        [InlineData(InsuranceType.ThirdPartyFireAndTheft, "BMW", "X5", 510)]
        [InlineData(InsuranceType.ThirdPartyFireAndTheft, "BMW", "5 Series", 400)]
        [InlineData(InsuranceType.ThirdPartyFireAndTheft, "Audi", "A4", 300)]
        [InlineData(InsuranceType.ThirdPartyOnly, "Ford", "Puma", 180)]
        [InlineData(InsuranceType.ThirdPartyOnly, "Audi", "A5", 250)]
        [InlineData(InsuranceType.ThirdPartyOnly, "BMW", "X5", 300)]
        public void GetBasePrice_MatchesOriginalPricing(InsuranceType type, string make, string model, decimal expected)
        {
            Assert.Equal(expected, _priceTable.GetBasePrice(type, make, model));
        }

        [Fact]
        public void GetBasePrice_ForUnlistedMake_ReturnsDefaultPrice()
        {
            Assert.Equal(PriceTable.DefaultPrice,
                _priceTable.GetBasePrice(InsuranceType.FullyComprehensive, "Toyota", "Yaris"));
        }

        [Fact]
        public void GetBasePrice_ForUnsupportedInsuranceType_Throws()
        {
            Assert.Throws<ArgumentOutOfRangeException>(
                () => _priceTable.GetBasePrice((InsuranceType)99, "Ford", "Focus"));
        }
    }
}
