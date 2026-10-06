using ExerciseApp.Model;

namespace ExerciseApp.Service
{
    public class VehicleCatalogue
    {
        private static readonly (string Make, string[] Models)[] Vehicles =
        [
            ("Ford", ["Fiesta", "Focus", "Puma", "S Max"]),
            ("Audi", ["A3", "A4", "A5"]),
            ("BMW", ["X5", "3 Series", "5 Series"]),
        ];

        public QuoteDetail GetQuoteDetail()
        {
            var quoteDetail = new QuoteDetail();

            foreach (var (make, models) in Vehicles)
            {
                quoteDetail.Makes.Add(make);

                var modelSpec = new ModelSpec { Make = make };
                modelSpec.Models.AddRange(models);
                quoteDetail.Models.Add(modelSpec);
            }

            return quoteDetail;
        }
    }
}
