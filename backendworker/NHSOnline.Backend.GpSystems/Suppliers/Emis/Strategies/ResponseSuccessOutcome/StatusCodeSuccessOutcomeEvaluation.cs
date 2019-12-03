using System.Collections.Generic;
using System.Net;

namespace NHSOnline.Backend.GpSystems.Suppliers.Emis.Strategies.ResponseSuccessOutcome
{
    public class StatusCodeSuccessOutcomeEvaluation : IResponseSuccessOutcomeStrategy
    {
        public bool isSuccess(List<HttpStatusCode> successStatusCodes, HttpStatusCode statusCode,
            bool isSuccessStatusCode, string stringResponse )
        {
            return isSuccessStatusCode || successStatusCodes.Contains(statusCode);
        }

        public bool populateErrors(List<HttpStatusCode> successStatusCodes, bool isSuccess, HttpStatusCode statusCode)
        {
            return !isSuccess || successStatusCodes.Contains(statusCode);
        }
    }
}