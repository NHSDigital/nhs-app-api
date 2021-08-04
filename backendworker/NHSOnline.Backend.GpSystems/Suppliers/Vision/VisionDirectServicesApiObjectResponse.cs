using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using NHSOnline.Backend.GpSystems.Suppliers.Vision.Models;
using NHSOnline.Backend.Support;
using NHSOnline.Backend.Support.AspNet.Filters;
using NHSOnline.Backend.Support.Http;
using NHSOnline.Backend.Support.ResponseParsers;

namespace NHSOnline.Backend.GpSystems.Suppliers.Vision
{
    public sealed class VisionDirectServicesApiObjectResponse<TBody> : ApiResponse
    {
        public VisionDirectServicesApiObjectResponse(HttpStatusCode statusCode) : base(statusCode)
        { }

        public async Task Parse(
            HttpResponseMessage responseMessage,
            IXmlResponseParser responseParser,
            ILogger logger)
        {
            var stringResponse = await GetStringResponse(responseMessage, logger);

            if (!String.IsNullOrEmpty(stringResponse))
            {
                ParseResponse(responseParser, logger, stringResponse);
            }
        }

        public VisionResponse<TBody> RawResponse { get; set; }

        public Detail ErrorDetail { get; set; }

        public TBody Body => RawResponse.ServiceContent;

        public string UnparseableResultMessage { get; private set; }

        public bool HasErrorResponse => !StatusCode.IsSuccessStatusCode()
                                        || FaultExists
                                        || UnparseableResultMessage != null;

        private VisionFault Fault => ErrorDetail?.VisionFault;

        public bool FaultExists => Fault != null;

        public override bool HasSuccessResponse => !HasErrorResponse;

        public string ErrorForLogging => $"fault: {JsonConvert.SerializeObject(Fault)}";

        protected override bool FormatResponseIfUnsuccessful => true;

        public bool IsInvalidRequestError =>
            (ErrorCategory ?? "").Contains("INVALID_REQUEST", StringComparison.Ordinal);

        public bool IsInvalidSecurityHeaderError =>
            (ErrorText ?? "").Contains("InvalidSecurity", StringComparison.Ordinal);

        public bool IsInvalidUserCredentialsError =>
            HasVisionApiErrorText(VisionApiErrorCodes.InvalidUserCredentials);

        public bool IsUnauthorisedResponse => StatusCode == HttpStatusCode.Unauthorized;

        public bool IsUnknownError => HasVisionApiErrorText(VisionApiErrorCodes.UnknownError);

        public bool IsAccessDeniedError => HasVisionApiErrorText(VisionApiErrorCodes.AccessDenied);

        private bool HasVisionApiErrorText(string visionErrorCode) =>
            visionErrorCode.Equals(ErrorText, StringComparison.Ordinal);

        /// <summary>
        /// For the previous SOAP calls, the error codes are in the <vision:code> element.
        /// For direct service calls, the error codes appear to be in the <vision:text> element.
        /// </summary>
        public string ErrorText => Fault?.Error?.Text;

        public string ErrorDescription => Fault?.Error?.Details;

        public string ErrorCategory => Fault?.Error?.Category;

        private void ParseResponse(
            IResponseParser responseParser,
            ILogger logger,
            string stringResponse)
        {
            if (String.IsNullOrEmpty(stringResponse))
            {
                logger.LogError("No response body");
                return;
            }

            if (!StatusCode.IsSuccessStatusCode())
            {
                TryParseError(stringResponse, responseParser, logger);
                return;
            }

            RawResponse = responseParser.ParseBody<VisionResponse<TBody>>(stringResponse);
        }

        private void TryParseError(string stringResponse, IResponseParser responseParser, ILogger logger)
        {
            try
            {
                ErrorDetail = responseParser.ParseBody<Detail>(stringResponse);
            }
            catch (NhsUnparsableException e)
            {
                logger.LogError(e, $"Vision Error Response could not be parsed: {stringResponse}");
                UnparseableResultMessage = stringResponse;
            }
        }
    }
}
