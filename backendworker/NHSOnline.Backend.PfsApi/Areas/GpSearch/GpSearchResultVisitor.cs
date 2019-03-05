using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NHSOnline.Backend.PfsApi.GpSearch.GpLookup;
using NHSOnline.Backend.PfsApi.GpSearch.Models;

namespace NHSOnline.Backend.PfsApi.Areas.GpSearch
{
    public class GpSearchResultVisitor : IGpSearchResultVisitor<IActionResult>
    {
        public IActionResult Visit(GpSearchResult.SuccessfullyRetrieved result)
        {
            return new OkObjectResult(result.Response);
        }

        public IActionResult Visit(GpSearchResult.Unsuccessful result)
        {
            return new StatusCodeResult(StatusCodes.Status500InternalServerError);
        }
        
        public IActionResult Visit(GpSearchResult.NhsSearchServiceUnavailable result)
        {
            return new StatusCodeResult(StatusCodes.Status502BadGateway);
        }
        
        public IActionResult Visit(GpSearchResult.BadRequest result)
        {
            return new BadRequestResult();
        }
    }
}