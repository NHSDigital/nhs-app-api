extern alias r4;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Support;
using NHSOnline.Backend.Support.Http;
using r4::Hl7.Fhir.Model;
using r4::Hl7.Fhir.Serialization;
using OperationOutcome = Hl7.Fhir.Model.OperationOutcome;

namespace NHSOnline.Backend.PfsApi.SecondaryCare
{
    public class SecondaryCareResponse : ApiResponse
    {
        private readonly FhirJsonParser _fhirParser;

        public SecondaryCareResponse(HttpStatusCode statusCode) : base(statusCode)
        {
            _fhirParser = new FhirJsonParser();
        }

        private const string Under16DiagnosticsErrorText = "UNDER_16_DENIED";

        public override bool HasSuccessResponse => StatusCode.IsSuccessStatusCode();

        public bool FailedToParseResponse { get; private set; }

        protected override bool FormatResponseIfUnsuccessful => true;

        public Bundle Body { get; private set; }

        public List<OperationOutcome.IssueComponent> Issues { get; private set; } =
            new List<OperationOutcome.IssueComponent>();

        public async Task<SecondaryCareResponse> Parse(HttpResponseMessage responseMessage, ILogger logger)
        {
            var stringResponse = await GetStringResponse(
                responseMessage,
                logger,
                "Secondary care request to Aggregator");

            return string.IsNullOrEmpty(stringResponse)
                ? this
                : await ParseResponse(logger, stringResponse);
        }

        private async Task<SecondaryCareResponse> ParseResponse(ILogger logger, string response)
        {
            if (string.IsNullOrEmpty(response))
            {
                return this;
            }

            try
            {
                Body = await _fhirParser.ParseAsync<Bundle>(response);

                Issues =
                    Body?.Entry
                        .Select(x => x.Resource)
                        .OfType<OperationOutcome>()
                        .SelectMany(x => x.Issue)
                        .ToList() ?? new List<OperationOutcome.IssueComponent>();
            }
            catch (Exception e)
            {
                logger.LogError(e, "Failed to parse Bundle response from Aggregator");
                FailedToParseResponse = true;
            }

            return this;
        }

        public bool IsUnder16Error()
        {
            return StatusCode == HttpStatusCode.Forbidden
                   && Issues.Any(x => x.Diagnostics == Under16DiagnosticsErrorText);
        }
    }
}