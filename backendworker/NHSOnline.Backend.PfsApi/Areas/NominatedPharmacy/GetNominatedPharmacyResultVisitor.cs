using System.Net;
using Microsoft.AspNetCore.Mvc;
using NHSOnline.Backend.NominatedPharmacy.Models;

namespace NHSOnline.Backend.PfsApi.Areas.NominatedPharmacy
{
    public class GetNominatedPharmacyResultVisitor : IGetNominatedPharmacyResultVisitor<IActionResult>
    {
        public IActionResult Visit(GetNominatedPharmacyResult.Success result)
        {
            return new OkObjectResult(new PharmacyDetailsResponse
            {
                PharmacyDetails = result.PharmacyDetails,
                NominatedPharmacyEnabled = true
            });
        }

        public IActionResult Visit(GetNominatedPharmacyResult.PersonalChecksFailed result)
        {
            return new OkObjectResult(new PharmacyDetailsResponse
            {
                NominatedPharmacyEnabled = false
            });
        }

        public IActionResult Visit(GetNominatedPharmacyResult.PharmacyChecksFailed result)
        {
            return new OkObjectResult(new PharmacyDetailsResponse { NominatedPharmacyEnabled = false });
        }

        public IActionResult Visit(GetNominatedPharmacyResult.PharmacyRetrievalFailure result)
        {
            return new StatusCodeResult(
                GetNominatedPharmacyResult.PharmacyRetrievalFailure.GetErrorStatusCode(
                    result.GetNominatedPharmacyResponse.HttpStatusCode));
        }

        public IActionResult Visit(GetNominatedPharmacyResult.NhsNumberSuperseded result)
        {
            return new OkObjectResult(new PharmacyDetailsResponse { NominatedPharmacyEnabled = false });
        }

        public IActionResult Visit(GetNominatedPharmacyResult.ConfidentialAccount result)
        {
            return new OkObjectResult(new PharmacyDetailsResponse { NominatedPharmacyEnabled = false });
        }

        public IActionResult Visit(GetNominatedPharmacyResult.InternalServerError result)
        {
            return new StatusCodeResult((int)HttpStatusCode.InternalServerError);
        }

        public IActionResult Visit(GetNominatedPharmacyResult.ConfigNotEnabled result)
        {
            return new OkObjectResult(new PharmacyDetailsResponse { NominatedPharmacyEnabled = false });
        }

        public IActionResult Visit(GetNominatedPharmacyResult.GpPracticeEpsNotEnabled result)
        {
            return new OkObjectResult(new PharmacyDetailsResponse { NominatedPharmacyEnabled = false });
        }

        public IActionResult Visit(GetNominatedPharmacyResult.GpPracticeFailure result)
        {
            return new StatusCodeResult(
                GetNominatedPharmacyResult.PharmacyRetrievalFailure.GetErrorStatusCode(result.StatusCode));
        }

        public IActionResult Visit(GetNominatedPharmacyResult.NoNominatedPharmacy result)
        {
            return new OkObjectResult(new PharmacyDetailsResponse { NominatedPharmacyEnabled = true });
        }

        public IActionResult Visit(GetNominatedPharmacyResult.PharmacyDetailFailure result)
        {
            return new StatusCodeResult(
                GetNominatedPharmacyResult.PharmacyRetrievalFailure.GetErrorStatusCode(result.StatusCode));
        }

        public IActionResult Visit(GetNominatedPharmacyResult.InvalidPharmacySubtype result)
        {
            return new OkObjectResult(new PharmacyDetailsResponse { NominatedPharmacyEnabled = false });
        }
    }
}