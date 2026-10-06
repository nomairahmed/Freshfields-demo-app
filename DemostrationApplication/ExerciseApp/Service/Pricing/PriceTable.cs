using System;
using System.Collections.Generic;
using ExerciseApp.Model;

namespace ExerciseApp.Service.Pricing
{
    /// <summary>
    /// Looks up a base price from the most specific match available:
    /// insurance type + make + model, then insurance type + make, then the default price.
    /// </summary>
    public class PriceTable : IBasePriceProvider
    {
        public const decimal DefaultPrice = 300;

        private const string AnyModel = null;

        private static readonly Dictionary<(InsuranceType InsuranceType, string Make, string Model), decimal> Prices = new()
        {
            [(InsuranceType.FullyComprehensive, "Ford", AnyModel)] = 200,
            [(InsuranceType.FullyComprehensive, "BMW", AnyModel)] = 400,
            [(InsuranceType.FullyComprehensive, "BMW", "X5")] = 500,

            [(InsuranceType.ThirdPartyFireAndTheft, "Ford", AnyModel)] = 180,
            [(InsuranceType.ThirdPartyFireAndTheft, "BMW", AnyModel)] = 400,
            [(InsuranceType.ThirdPartyFireAndTheft, "BMW", "X5")] = 510,

            [(InsuranceType.ThirdPartyOnly, "Ford", AnyModel)] = 180,
            [(InsuranceType.ThirdPartyOnly, "Audi", AnyModel)] = 250,
        };

        public decimal GetBasePrice(InsuranceType insuranceType, string make, string model)
        {
            if (!Enum.IsDefined(insuranceType))
                throw new ArgumentOutOfRangeException(nameof(insuranceType), insuranceType, "Unsupported insurance type.");

            if (Prices.TryGetValue((insuranceType, make, model), out var price))
                return price;

            if (Prices.TryGetValue((insuranceType, make, AnyModel), out price))
                return price;

            return DefaultPrice;
        }
    }
}
