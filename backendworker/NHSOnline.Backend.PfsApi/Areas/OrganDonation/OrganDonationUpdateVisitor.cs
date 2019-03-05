using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NHSOnline.Backend.PfsApi.OrganDonation;

namespace NHSOnline.Backend.PfsApi.Areas.OrganDonation
{
    public class OrganDonationUpdatedVisitor : IOrganDonationRegistrationResultVisitor<IActionResult>
    {
        
        public IActionResult Visit(OrganDonationRegistrationResult.SuccessfullyRegistered result)
        {
            return new OkObjectResult(result.Response);
        }
        
        public IActionResult Visit(OrganDonationRegistrationResult.SystemError result)
        {
            return new StatusCodeResult(StatusCodes.Status500InternalServerError);
        }
        
        public IActionResult Visit(OrganDonationRegistrationResult.UpstreamError result)
        {
            return new StatusCodeResult(StatusCodes.Status502BadGateway);
        }
        
        public IActionResult Visit(OrganDonationRegistrationResult.Timeout result)
        {
            return new StatusCodeResult(StatusCodes.Status504GatewayTimeout);
        }
    }
}
