using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.GpSystems.Suppliers.Emis.Strategies.ResponseSuccessOutcome;
using NHSOnline.Backend.Support.ResponseParsers;

namespace NHSOnline.Backend.GpSystems.Suppliers.Emis
{
    public class EmisApiObjectResponse<TBody> : EmisApiResponse
    {
        public TBody Body { get; set; }
        private RequestsForSuccessOutcome RequestType { get; }
        private List<HttpStatusCode> StatusCodes { get; }

        public EmisApiObjectResponse(
            HttpStatusCode statusCode,
            RequestsForSuccessOutcome typeOfRequest,
            List<HttpStatusCode> statusCodes) : base(statusCode)
        {
            RequestType = typeOfRequest;
            StatusCodes = statusCodes;
        }

        public async Task Parse(
            HttpResponseMessage responseMessage,
            IJsonResponseParser responseParser,
            ILogger logger)
        {
            var stringResponse = await GetStringResponse(responseMessage, logger);
            RawResponse = stringResponse;
            if (!String.IsNullOrEmpty(stringResponse))
            {
                ParseResponse(responseParser, stringResponse, responseMessage, logger);
            }
        }

        private void ParseResponse(
            IResponseParser responseParser,
            string stringResponse,
            HttpResponseMessage responseMessage,
            ILogger logger)
        {
            if (String.IsNullOrEmpty(stringResponse))
            {
                logger.LogError("No response body");
                return;
            }

            var successStrategy = GetOutcomeEvaluator(RequestType);
            Body = successStrategy.IsSuccess(StatusCodes, StatusCode,
                responseMessage.IsSuccessStatusCode, stringResponse) ? responseParser.ParseBody<TBody>(stringResponse) : default;

            if (successStrategy.PopulateErrors(StatusCodes, responseMessage.IsSuccessStatusCode, StatusCode))
            {
                StandardErrorResponse = ParseBadRequestError<StandardErrorResponse>(
                    responseParser, stringResponse, responseMessage);
                ErrorResponseBadRequest = ParseBadRequestError<BadRequestErrorResponse>(
                    responseParser, stringResponse, responseMessage);
                ExceptionErrorResponse = responseParser.ParseError<ExceptionErrorResponse>(
                    stringResponse,
                    responseMessage,
                    HttpStatusCode.BadRequest);
            }
            else
            {
                StandardErrorResponse = default;
                ErrorResponseBadRequest = default;
                ExceptionErrorResponse = default;
            }
        }
        protected override bool FormatResponseIfUnsuccessful => true;

        private static T ParseBadRequestError<T>(
            IResponseParser responseParser,
            string stringResponse, HttpResponseMessage responseMessage)
        {
            if (responseMessage.StatusCode == HttpStatusCode.BadRequest)
            {
                return responseParser.ParseBody<T>(stringResponse);
            }

            return default;
        }

        private static IResponseSuccessOutcomeStrategy GetOutcomeEvaluator(RequestsForSuccessOutcome requestType)
            => requestType switch
            {
                RequestsForSuccessOutcome.PatientMessageDetailsGet => new RegexSuccessOutcomeEvaluation(),
                RequestsForSuccessOutcome.VerificationPost => new StatusCodeSuccessOutcomeEvaluation(),
                RequestsForSuccessOutcome.MedicalRecordGet => new StatusCodeSuccessOutcomeEvaluation(),
                _ => new DefaultSuccessOutcomeEvaluation()
            };
    }
}