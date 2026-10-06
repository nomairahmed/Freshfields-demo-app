namespace ExerciseApp.Model
{
    public class QuoteResponse
    {
       public bool QuoteRequestValid { get; set; }
       public decimal Quote{ get; set; }
       public bool Declined { get; set; }
       public string DeclineReason { get; set; }
       public string Reference { get; set; }
    }
}
