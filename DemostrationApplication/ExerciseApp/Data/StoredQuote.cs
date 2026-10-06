using System;
using ExerciseApp.Model;

namespace ExerciseApp.Data
{
    public class StoredQuote
    {
        public int Id { get; set; }
        public string Reference { get; set; }
        public DateTime CreatedUtc { get; set; }
        public DateOnly DateOfBirth { get; set; }
        public string Make { get; set; }
        public string Model { get; set; }
        public InsuranceType InsuranceType { get; set; }
        public decimal Premium { get; set; }
    }
}
