using System.Net;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace NHSOnline.HttpMocks.Spine
{
    [Route("spine/ORD/2-0-0/organisations")]
    public sealed class OrganisationsController: ControllerBase
    {
        private readonly SpineOrganisationsResponse _organisationsResponse;

        public OrganisationsController(SpineOrganisationsResponse organisationsResponse)
        {
            _organisationsResponse = organisationsResponse;
        }

        [HttpGet]
        public IActionResult Organisations([FromQuery] string primaryRoleId, [FromQuery] int? offset)
        {
            var results = offset == 1000 ? _organisationsResponse.SearchResultsExceedingLimit():
                _organisationsResponse.SearchResults();
            Response.Headers.Add("X-Total-Count", _organisationsResponse.TotalCount().ToString());
            var resultsJson = results.ToString(Formatting.None);
            return new ContentResult
            {
                Content = resultsJson,
                ContentType = "application/json",
                StatusCode = (int)HttpStatusCode.OK
            };
        }

        [HttpGet]
        [Route("{odsCode}")]
        public IActionResult Organisation(string odsCode)
        {
            if (_organisationsResponse.TryGetByOdsCode(odsCode, out SpineOrganisation organisation))
            {
                var resultsJson = organisation.Details().ToString(Formatting.None);
                return new ContentResult
                {
                    Content = resultsJson,
                    ContentType = "application/json",
                    StatusCode = (int)HttpStatusCode.OK
                };
            }
            return NotFound();
        }
    }
}
