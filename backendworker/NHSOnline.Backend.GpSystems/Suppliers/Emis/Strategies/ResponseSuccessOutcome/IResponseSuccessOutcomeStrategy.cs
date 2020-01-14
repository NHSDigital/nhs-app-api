using System.Collections.Generic;
using System.Net;

namespace NHSOnline.Backend.GpSystems.Suppliers.Emis.Strategies.ResponseSuccessOutcome
{
    public interface IResponseSuccessOutcomeStrategy
    {
        bool IsSuccess( List<HttpStatusCode> successStatusCodes, HttpStatusCode statusCode,
            bool isSuccessStatusCode, string stringResponse);

        bool PopulateErrors(List<HttpStatusCode> successStatusCodes, bool isSuccess, HttpStatusCode statusCode);
    }
}