using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using NHSOnline.HttpMocks.SecondaryCare.Builders;
using Hl7.Fhir.Model;
using Hl7.Fhir.Serialization;
using Microsoft.AspNetCore.Http;
using FhirPatient = Hl7.Fhir.Model.Patient;

namespace NHSOnline.HttpMocks.SecondaryCare
{
    [Route("patient-care-aggregator-api/aggregator")]
    public class PatientCareAggregatorController : Controller
    {
        private readonly FhirJsonSerializer _serializer;

        public PatientCareAggregatorController()
        {
            _serializer = new FhirJsonSerializer();
        }

        [HttpGet]
        [Route("events")]
        public async Task<IActionResult> GetSecondaryCareSummaryResponseForNhsNumber(
            [FromQuery(Name="patient:identifier")] string nhsNumberQuery)
        {
            var nhsNumber = !string.IsNullOrEmpty(nhsNumberQuery)
                ? nhsNumberQuery.Split('|')[1]
                : string.Empty;

            if (string.IsNullOrEmpty(nhsNumber))
            {
                return BadRequest("No NHS Number provided");
            }

            if (string.Equals("2092013752", nhsNumber, StringComparison.Ordinal))
            {
                throw new NotImplementedException("Deliberately thrown.");
            }

            var errors = GetOperationOutcomeErrorsFromNhsNumber(nhsNumber);
            if (string.Equals("2090220899", nhsNumber, StringComparison.Ordinal))
            {
                // NHS Number 929 022 0899 (user 13) results results in 403 forbidden due to < 16 years old.
                Response.StatusCode = StatusCodes.Status403Forbidden;
                var forbiddenResponse = await _serializer.SerializeToStringAsync((new OperationOutcome
                {
                    Issue = errors.ToList()
                }));

                return new ObjectResult(forbiddenResponse);
            }

            var referrals = GetReferralsFromNhsNumber(nhsNumber);
            var upcomingAppointments = GetUpcomingAppointmentsFromNhsNumber(nhsNumber);

            var response = await _serializer.SerializeToStringAsync(new SecondaryCareSummaryBuilder()
                .ForNhsNumber(nhsNumber)
                .WithReferrals(referrals)
                .WithUpcomingAppointments(upcomingAppointments)
                .WithPartialErrors(errors)
                .Build());

            return new OkObjectResult(response);
        }

        private static IEnumerable<CarePlan.ActivityComponent> GetReferralsFromNhsNumber(string nhsNumber)
        {
            return StubbedReferralResponses.ReferralMapping[nhsNumber];
        }

        private IEnumerable<CarePlan.ActivityComponent> GetUpcomingAppointmentsFromNhsNumber(string nhsNumber)
        {
            return StubbedUpcomingAppointmentResponses.UpcomingAppointmentMapping[nhsNumber];
        }

        private IEnumerable<OperationOutcome.IssueComponent> GetOperationOutcomeErrorsFromNhsNumber(string nhsNumber)
        {
            return !StubbedPartialErrorResponses.PartialErrorMapping.ContainsKey(nhsNumber)
                ? new List<OperationOutcome.IssueComponent>()
                : StubbedPartialErrorResponses.PartialErrorMapping[nhsNumber];
        }
    }
}
