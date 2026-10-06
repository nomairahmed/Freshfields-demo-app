using System.Collections.Generic;
using ExerciseApp.Model;
using ExerciseApp.Service.Pricing;
using ExerciseApp.Service.Rules;

namespace ExerciseApp.Service
{
    public class QuoteService(IBasePriceProvider prices, IEnumerable<IQuoteRule> rules)
    {
        public QuoteResult PerformQuote(QuoteRequest request)
        {
            foreach (var rule in rules)
            {
                var reason = rule.GetDeclineReason(request);
                if (reason is not null)
                    return QuoteResult.Declined(reason);
            }

            return QuoteResult.Accepted(
                prices.GetBasePrice(request.InsuranceType.Value, request.Make, request.Model));
        }
    }
}
