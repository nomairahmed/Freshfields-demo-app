using System;
using ExerciseApp.Data;
using ExerciseApp.Helpers;

namespace ExerciseApp.Model
{
    public class SavedQuoteResponse
    {
        public string Reference { get; set; }
        public DateTime CreatedUtc { get; set; }
        public DateOnly DateOfBirth { get; set; }
        public string Make { get; set; }
        public string Model { get; set; }
        public InsuranceType InsuranceType { get; set; }
        public string InsuranceTypeDescription { get; set; }
        public decimal Premium { get; set; }

        public static SavedQuoteResponse From(StoredQuote quote) => new()
        {
            Reference = quote.Reference,
            CreatedUtc = DateTime.SpecifyKind(quote.CreatedUtc, DateTimeKind.Utc),
            DateOfBirth = quote.DateOfBirth,
            Make = quote.Make,
            Model = quote.Model,
            InsuranceType = quote.InsuranceType,
            InsuranceTypeDescription = quote.InsuranceType.GetEnumDescription(),
            Premium = quote.Premium
        };
    }
}
