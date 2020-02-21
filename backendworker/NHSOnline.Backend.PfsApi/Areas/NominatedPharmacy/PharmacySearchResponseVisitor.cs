using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NHSOnline.Backend.PfsApi.Areas.NominatedPharmacy.Models;
using NHSOnline.Backend.PfsApi.GpSearch.Models.Pharmacy;

namespace NHSOnline.Backend.PfsApi.Areas.NominatedPharmacy
{
    public class PharmacySearchResponseVisitor : IPharmacySearchResponseVisitor<IActionResult>
    {
        public IActionResult Visit(PharmacySearchResult.Success result)
        {
            return new OkObjectResult(result.Response);
        }

        public IActionResult Visit(PharmacySearchResult.InvalidPostcode result)
        {
            return new OkObjectResult(new PharmacySearchResultResponse());
        }

        public IActionResult Visit(PharmacySearchResult.PostcodeResultFailure result)
        {
            return new StatusCodeResult((int)result.StatusCode); 
        }
        
        public IActionResult Visit(PharmacySearchResult.BadRequest result)
        {
            return new StatusCodeResult(StatusCodes.Status400BadRequest);
        }
        
        public IActionResult Visit(PharmacySearchResult.InternalServerError result)
        {
            return new StatusCodeResult(StatusCodes.Status500InternalServerError);
        }
        
        public IActionResult Visit(PharmacySearchResult.ModelValidationError result)
        {
            return new StatusCodeResult(StatusCodes.Status400BadRequest);
        }
    }
}