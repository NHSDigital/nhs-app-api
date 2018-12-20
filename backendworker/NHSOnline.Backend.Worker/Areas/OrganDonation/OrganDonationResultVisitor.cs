using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NHSOnline.Backend.Worker.OrganDonation;

namespace NHSOnline.Backend.Worker.Areas.OrganDonation
{
    public class OrganDonationResultVisitor : IOrganDonationResultVisitor<IActionResult>
    {
        public IActionResult Visit(OrganDonationResult.NewRegistration result)
        {
            return new OkObjectResult(result.Registration);
        }

        public IActionResult Visit(OrganDonationResult.ExistingRegistration result)
        {
            return new OkObjectResult(result.Registration);
        }

        public IActionResult Visit(OrganDonationResult.DemographicsRetrievalFailed result)
        {
            return new StatusCodeResult(StatusCodes.Status500InternalServerError);
        }

        public IActionResult Visit(OrganDonationResult.DemographicsForbidden result)
        {
            return new StatusCodeResult(StatusCodes.Status403Forbidden);
        }

        public IActionResult Visit(OrganDonationResult.DemographicsInternalServerError result)
        {
            return new StatusCodeResult(StatusCodes.Status500InternalServerError);
        }

        public IActionResult Visit(OrganDonationResult.DemographicsBadGateway result)
        {
            return new StatusCodeResult(StatusCodes.Status502BadGateway);
        }

        public IActionResult Visit(OrganDonationResult.DuplicateRecord result)
        {
            return new StatusCodeResult(StatusCodes.Status409Conflict);
        }

        public IActionResult Visit(OrganDonationResult.SearchSystemUnavailable result)
        {
            return new StatusCodeResult(StatusCodes.Status502BadGateway);
        }

        public IActionResult Visit(OrganDonationResult.BadSearchRequest result)
        {
            return new StatusCodeResult(StatusCodes.Status500InternalServerError);
        }

        public IActionResult Visit(OrganDonationResult.SearchTimeout result)
        {
            return new StatusCodeResult(StatusCodes.Status504GatewayTimeout);
        }

        public IActionResult Visit(OrganDonationResult.SearchError result)
        {
            return new StatusCodeResult(StatusCodes.Status502BadGateway);
        }
    }
}
