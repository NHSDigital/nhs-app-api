using System.Collections.Generic;
using System.Net;

namespace NHSOnline.Backend.GpSystems.Suppliers.Emis.Strategies.ResponseSuccessOutcome
{
    public interface IResponseSuccessOutcomeStrategy
    {
        bool isSuccess( List<HttpStatusCode> successStatusCodes, HttpStatusCode statusCode,
            bool isSuccessStatusCode, string stringResponse);

        bool populateErrors(List<HttpStatusCode> successStatusCodes, bool isSuccess, HttpStatusCode statusCode);
    }
}