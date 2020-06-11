using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using NHSOnline.Backend.GpSystems.Suppliers.Vision.Models;
using NHSOnline.Backend.Support;
using NHSOnline.Backend.Support.Http;
using NHSOnline.Backend.Support.ResponseParsers;

namespace NHSOnline.Backend.GpSystems.Suppliers.Vision
{
    public sealed class VisionPfsApiObjectResponse<TBody> : ApiResponse
    {
        // Note
        // We don't know whether Vision always populates certain properties when there is an error.
        // So there are a lot of null checks below using the null conditional operator.

        public VisionPfsApiObjectResponse(HttpStatusCode statusCode) : base(statusCode)
        { }

        public async Task<VisionPfsApiObjectResponse<TBody>> Parse(
            HttpResponseMessage responseMessage,
            IXmlResponseParser responseParser,
            ILogger logger)
        {
            var stringResponse = await GetStringResponse(responseMessage, logger);
            return string.IsNullOrEmpty(stringResponse)
                ? this : ParseResponse(responseParser, logger, stringResponse);
        }

        public VisionResponseEnvelope<TBody> RawResponse { get; set; }

        public string UnparsableResultMessage { get; set; }

        public TBody Body => RawResponse.Body.VisionResponse.ServiceContent;

        public bool HasErrorResponse => !StatusCode.IsSuccessStatusCode()
                                        || FaultExists
                                        || OutcomeUnsuccessful
                                        || UnparsableResultMessage != null;

        private Fault Fault => RawResponse?.Body?.Fault;
        private Outcome Outcome => RawResponse?.Body?.VisionResponse?.ServiceHeader?.Outcome;

        public bool FaultExists => Fault != null;

        private bool OutcomeUnsuccessful => bool.FalseString.Equals(
            Outcome?.Successful,
            StringComparison.OrdinalIgnoreCase);

        public override bool HasSuccessResponse => !HasErrorResponse;

        public string ErrorForLogging => $"fault: {JsonConvert.SerializeObject(Fault)}, " +
                                         $"error: {JsonConvert.SerializeObject(Outcome?.Error)}";

        protected override bool FormatResponseIfUnsuccessful => true;

        public bool IsInvalidRequestError =>
            (Fault?.Detail?.VisionFault?.Error?.Category ?? "").Contains("INVALID_REQUEST",
                StringComparison.Ordinal);

        public bool IsInvalidSecurityHeaderError =>
            (Fault?.FaultCode ?? "").Contains("InvalidSecurity", StringComparison.Ordinal);

        public bool IsInvalidUserCredentialsError =>
            HasVisionApiErrorCode(VisionApiErrorCodes.InvalidUserCredentials);

        public bool IsUnknownError => HasVisionApiErrorCode(VisionApiErrorCodes.UnknownError);

        public bool IsAccessDeniedError => HasVisionApiErrorCode(VisionApiErrorCodes.AccessDenied);

        public bool IsAppointmentSlotNotBookedToCurrentUserError =>
            HasVisionApiErrorCode(VisionApiErrorCodes.AppointmentSlotNotBookedToCurrentUser);

        public bool IsAppointmentSlotNotFoundError =>
            HasVisionApiErrorCode(VisionApiErrorCodes.AppointmentSlotNotFound);

        public bool IsAppointmentSlotAlreadyBookedError =>
            HasVisionApiErrorCode(VisionApiErrorCodes.AppointmentSlotAlreadyBooked);

        public bool IsAppointmentBookingLimitReachedError =>
            HasVisionApiErrorCode(VisionApiErrorCodes.AppointmentBookingLimitReached);

        private bool HasVisionApiErrorCode(string visionErrorCode)
        {
            return visionErrorCode.Equals(Outcome?.Error?.Code, StringComparison.Ordinal);
        }
        public string ErrorCode => Outcome?.Error?.Code;
        public string ErrorMessage => Outcome?.Error?.Description;

        private VisionPfsApiObjectResponse<TBody> ParseResponse(
            IResponseParser responseParser,
            ILogger logger,
            string stringResponse)
        {
            try
            {
                RawResponse = responseParser.ParseBody<VisionResponseEnvelope<TBody>>(stringResponse);
            }
            catch (InvalidOperationException e)
            {
                logger.LogError(e, $"Vision Error Response could not be parsed: : {stringResponse}");
                UnparsableResultMessage = stringResponse;
            }

            return this;
        }
    }
}