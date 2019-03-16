using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NHSOnline.Backend.PfsApi.OrganDonation;

namespace NHSOnline.Backend.PfsApi.Areas.OrganDonation
{
    public class OrganDonationWithdrawResultVisitor : IOrganDonationWithdrawResultVisitor<IActionResult>
    {
        
        public IActionResult Visit(OrganDonationWithdrawResult.SuccessfullyWithdrawn result)
        {
            return new StatusCodeResult(StatusCodes.Status200OK);
        }
        
        public IActionResult Visit(OrganDonationWithdrawResult.SystemError result)
        {
            return new StatusCodeResult(StatusCodes.Status500InternalServerError);
        }
        
        public IActionResult Visit(OrganDonationWithdrawResult.UpstreamError result)
        {
            return new ObjectResult(result.Response)
            {
                StatusCode = StatusCodes.Status502BadGateway                
            };
        }
        
        public IActionResult Visit(OrganDonationWithdrawResult.Timeout result)
        {
            return new StatusCodeResult(StatusCodes.Status502BadGateway);
        }
    }
}
