using System.Linq;
using ExerciseApp.Service;
using Xunit;

namespace ExerciseApp.Tests
{
    public class VehicleCatalogueTests
    {
        private readonly VehicleCatalogue _catalogue = new();

        [Fact]
        public void EveryMake_HasAtLeastOneModel()
        {
            var detail = _catalogue.GetQuoteDetail();

            foreach (var make in detail.Makes)
            {
                var spec = Assert.Single(detail.Models, m => m.Make == make);
                Assert.NotEmpty(spec.Models);
            }
        }

        [Fact]
        public void EveryModelList_BelongsToAListedMake()
        {
            var detail = _catalogue.GetQuoteDetail();

            Assert.All(detail.Models, spec => Assert.Contains(spec.Make, detail.Makes));
        }

        [Fact]
        public void BmwModels_AreAvailable()
        {
            var detail = _catalogue.GetQuoteDetail();

            var bmw = detail.Models.Single(m => m.Make == "BMW");
            Assert.Equal(new[] { "X5", "3 Series", "5 Series" }, bmw.Models);
        }
    }
}
