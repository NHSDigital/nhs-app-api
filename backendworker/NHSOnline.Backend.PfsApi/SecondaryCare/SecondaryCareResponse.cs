extern alias r4;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using NHSOnline.Backend.Support;
using NHSOnline.Backend.Support.Http;
using r4::Hl7.Fhir.Model;
using r4::Hl7.Fhir.Serialization;

namespace NHSOnline.Backend.PfsApi.SecondaryCare
{
    public class SecondaryCareResponse : ApiResponse
    {
        private readonly FhirJsonParser _fhirParser;

        public SecondaryCareResponse(HttpStatusCode statusCode) : base(statusCode)
        {
            _fhirParser = new FhirJsonParser();
        }

        public override bool HasSuccessResponse => StatusCode.IsSuccessStatusCode();
        public bool FailedToParseResponse { get; private set; }

        protected override bool FormatResponseIfUnsuccessful => false;

        public CarePlan Body { get; private set; }

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

        private async Task<SecondaryCareResponse> ParseResponse(ILogger logger, string stringResponse)
        {
            if (!HasSuccessResponse)
            {
                return this;
            }

            var response = JsonConvert.DeserializeObject<List<object>>(stringResponse);
            if (response is null)
            {
                logger.LogError("Failed to parse response to list from Aggregator");
                FailedToParseResponse = true;

                return this;
            }

            try
            {
                Body = await _fhirParser.ParseAsync<CarePlan>(response[0].ToString());
            }
            catch (Exception e)
            {
                logger.LogError(e, "Failed to parse CarePlan from Aggregator");
                FailedToParseResponse = true;
            }

            return this;
        }
    }
}