using ExerciseApp.Model;

namespace ExerciseApp.Service.Pricing
{
    public interface IBasePriceProvider
    {
        decimal GetBasePrice(InsuranceType insuranceType, string make, string model);
    }
}
