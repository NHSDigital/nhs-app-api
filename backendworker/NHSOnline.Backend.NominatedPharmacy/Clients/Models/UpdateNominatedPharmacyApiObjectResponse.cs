using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.NominatedPharmacy.Soap;
using NHSOnline.Backend.Support;
using NHSOnline.Backend.Support.Http;
using NHSOnline.Backend.Support.ResponseParsers;

namespace NHSOnline.Backend.NominatedPharmacy.Clients.Models
{
    public class UpdateNominatedPharmacyApiObjectResponse : NominatedPharmacyApiResponse
    {
        public string RawResponse { get; private set; }

        public UpdateNominatedPharmacyResponseEnvelope Response { get; set; }
        
        public UpdateNominatedPharmacyApiObjectResponse(HttpStatusCode statusCode) : base(statusCode)
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
                RawResponse = stringResponse;

                Response = responseParser.ParseBody<UpdateNominatedPharmacyResponseEnvelope>(stringResponse);
            }
        }

        protected override bool FormatResponseIfUnsuccessful => false;
    }
    
    public abstract class UpdateNominatedPharmacyApiResponse : ApiResponse
    {
        protected UpdateNominatedPharmacyApiResponse(HttpStatusCode statusCode) : base(statusCode)
        {
        }

        public override bool HasSuccessResponse => StatusCode.IsSuccessStatusCode();
        public override string ErrorForLogging => $"Error Code: '{StatusCode}'.";
    }
}

