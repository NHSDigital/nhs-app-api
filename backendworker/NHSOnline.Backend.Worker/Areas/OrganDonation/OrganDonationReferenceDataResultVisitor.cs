using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NHSOnline.Backend.Worker.OrganDonation;

namespace NHSOnline.Backend.Worker.Areas.OrganDonation
{
    public class OrganDonationReferenceDataResultVisitor : IOrganDonationReferenceDataResultVisitor<IActionResult>
    {
        public IActionResult Visit(OrganDonationReferenceDataResult.SuccessfullyRetrieved result)
        {
            return new OkObjectResult(result.Response);
        }
        
        public IActionResult Visit(OrganDonationReferenceDataResult.SystemError result)
        {
            return new StatusCodeResult(StatusCodes.Status500InternalServerError);
        }
        
        public IActionResult Visit(OrganDonationReferenceDataResult.UpstreamError result)
        {
            return new StatusCodeResult(StatusCodes.Status502BadGateway);
        }
        
        public IActionResult Visit(OrganDonationReferenceDataResult.Timeout result)
        {
            return new StatusCodeResult(StatusCodes.Status504GatewayTimeout);
        }
    }
}
