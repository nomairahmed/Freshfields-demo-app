using System;
using System.Threading.Tasks;
using ExerciseApp.Data;
using ExerciseApp.Model;
using ExerciseApp.Service;
using Microsoft.AspNetCore.Mvc;

namespace ExerciseApp.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class QuoteController(
        QuoteService quoteService,
        VehicleCatalogue vehicleCatalogue,
        IQuoteRepository quoteRepository,
        TimeProvider clock) : ControllerBase
    {
        [HttpGet]
        public QuoteDetail Get()
        {
            return vehicleCatalogue.GetQuoteDetail();
        }

        [HttpPost]
        public async Task<QuoteResponse> Post(QuoteRequest request)
        {
            var returnObject = new QuoteResponse() { QuoteRequestValid = false };
            if (TryValidateModel(request))
            {
                var result = quoteService.PerformQuote(request);
                returnObject.QuoteRequestValid = true;
                returnObject.Quote = result.Premium;
                returnObject.Declined = result.IsDeclined;
                returnObject.DeclineReason = result.DeclineReason;

                if (!result.IsDeclined)
                {
                    var saved = await quoteRepository.AddAsync(new StoredQuote
                    {
                        CreatedUtc = clock.GetUtcNow().UtcDateTime,
                        DateOfBirth = DateOnly.FromDateTime(request.DateOfBirth.Value),
                        Make = request.Make,
                        Model = request.Model,
                        InsuranceType = request.InsuranceType.Value,
                        Premium = result.Premium
                    });
                    returnObject.Reference = saved.Reference;
                }
            }
            
            return returnObject;
        }

        [HttpGet("{reference}")]
        public async Task<ActionResult<SavedQuoteResponse>> GetByReference(string reference)
        {
            var quote = await quoteRepository.GetByReferenceAsync(QuoteReference.Normalise(reference));
            if (quote is null)
                return NotFound();

            return SavedQuoteResponse.From(quote);
        }
    }
}
