using ExerciseApp.Service;
using Xunit;

namespace ExerciseApp.Tests
{
    public class QuoteReferenceTests
    {
        [Fact]
        public void Generate_ProducesReferencesOfTheExpectedLengthAndCharacters()
        {
            var reference = QuoteReference.Generate();

            Assert.Equal(QuoteReference.Length, reference.Length);
            Assert.All(reference, c => Assert.Contains(c, QuoteReference.Alphabet));
        }

        [Theory]
        [InlineData("abcd2345", "ABCD2345")]
        [InlineData("  ABCD2345 ", "ABCD2345")]
        public void Normalise_TrimsAndUppercases(string input, string expected)
        {
            Assert.Equal(expected, QuoteReference.Normalise(input));
        }
    }
}
