using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.NominatedPharmacy.Soap;
using NHSOnline.Backend.Support.Http;
using NHSOnline.Backend.Support.ResponseParsers;

namespace NHSOnline.Backend.NominatedPharmacy.Clients.Models
{
    public class NominatedPharmacyApiObjectResponse<TBody> : NominatedPharmacyApiResponse
    {
        public NominatedPharmacyResponseEnvelope<TBody> RawResponse { get; set; }

        public TBody Body => RawResponse.Body.RetrievalQueryResponse;

        public NominatedPharmacyApiObjectResponse(HttpStatusCode statusCode) : base(statusCode)
        {
        }

        public async Task Parse(
            HttpResponseMessage responseMessage,
            IXmlResponseParser responseParser,
            ILogger logger)
        {
            var stringResponse = await GetStringResponse(responseMessage, logger);
            if (!string.IsNullOrEmpty(stringResponse))
            {
                RawResponse = responseParser.ParseBody<NominatedPharmacyResponseEnvelope<TBody>>(
                    stringResponse);
            }
        }

        protected override bool FormatResponseIfUnsuccessful => false;
    }
    
    public abstract class NominatedPharmacyApiResponse : ApiResponse
    {
        protected NominatedPharmacyApiResponse(HttpStatusCode statusCode) : base(statusCode)
        {
        }

        public override bool HasSuccessResponse => StatusCode.IsSuccessStatusCode();
        public override string ErrorForLogging => $"Error Code: '{StatusCode}'.";
    }
}

