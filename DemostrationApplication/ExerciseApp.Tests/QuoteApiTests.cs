using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using ExerciseApp.Data;
using ExerciseApp.Model;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace ExerciseApp.Tests
{
    public class QuoteApiTests(QuoteApiFactory factory) : IClassFixture<QuoteApiFactory>
    {
        private static readonly JsonSerializerOptions Json = new(JsonSerializerDefaults.Web)
        {
            Converters = { new JsonStringEnumConverter() }
        };

        private readonly HttpClient _client = factory.CreateClient();

        private static CancellationToken Cancellation => TestContext.Current.CancellationToken;

        [Fact]
        public async Task AcceptedQuote_IsSaved_AndCanBeRetrievedByItsReference()
        {
            var quote = await PostQuote("1990-01-01", "BMW", "X5", "FullyComprehensive");

            Assert.False(quote.Declined);
            Assert.Equal(500, quote.Quote);
            Assert.Matches("^[A-HJ-NP-Z2-9]{8}$", quote.Reference);

            var saved = await _client.GetFromJsonAsync<SavedQuoteResponse>($"/Quote/{quote.Reference}", Json, Cancellation);

            Assert.Equal(quote.Reference, saved.Reference);
            Assert.Equal(new DateOnly(1990, 1, 1), saved.DateOfBirth);
            Assert.Equal("BMW", saved.Make);
            Assert.Equal("X5", saved.Model);
            Assert.Equal(InsuranceType.FullyComprehensive, saved.InsuranceType);
            Assert.Equal("Fully Comprehensive", saved.InsuranceTypeDescription);
            Assert.Equal(500, saved.Premium);
            Assert.Equal(factory.Clock.GetUtcNow().UtcDateTime, saved.CreatedUtc);
        }

        [Fact]
        public async Task Retrieval_IgnoresTheCaseOfTheReference()
        {
            var quote = await PostQuote("1990-01-01", "Ford", "Focus", "ThirdPartyOnly");

            var response = await _client.GetAsync($"/Quote/{quote.Reference.ToLowerInvariant()}", Cancellation);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task DeclinedQuote_HasNoReference_AndIsNotSaved()
        {
            var countBefore = CountSavedQuotes();

            var quote = await PostQuote("2015-01-01", "Ford", "Focus", "FullyComprehensive");

            Assert.True(quote.Declined);
            Assert.Null(quote.Reference);
            Assert.Equal(countBefore, CountSavedQuotes());
        }

        [Fact]
        public async Task UnknownReference_ReturnsNotFound()
        {
            var response = await _client.GetAsync("/Quote/ZZZZZZZZ", Cancellation);

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task IncompleteRequest_ReturnsBadRequest()
        {
            var response = await _client.PostAsJsonAsync("/Quote", new { dateOfBirth = "1990-01-01", make = "Ford" }, Cancellation);

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        private async Task<QuoteResponse> PostQuote(string dateOfBirth, string make, string model, string insuranceType)
        {
            var response = await _client.PostAsJsonAsync("/Quote", new { dateOfBirth, make, model, insuranceType }, Cancellation);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<QuoteResponse>(Json, Cancellation);
        }

        private int CountSavedQuotes()
        {
            using var scope = factory.Services.CreateScope();
            return scope.ServiceProvider.GetRequiredService<QuoteDbContext>().Quotes.Count();
        }
    }
}
