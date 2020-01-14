using System.Collections.Generic;
using System.Net;
using System.Text.RegularExpressions;

namespace NHSOnline.Backend.GpSystems.Suppliers.Emis.Strategies.ResponseSuccessOutcome
{
    public class RegexSuccessOutcomeEvaluation : IResponseSuccessOutcomeStrategy
    {
        private static readonly Regex ErrorRegex = new Regex("\"ErrorCode\":|\"Exceptions\":\\[");
        
        public bool IsSuccess(List<HttpStatusCode> successStatusCodes, HttpStatusCode statusCode, bool isSuccessStatusCode, string stringResponse)
        {
            return isSuccessStatusCode && !ErrorRegex.IsMatch(stringResponse);
        }

        public bool PopulateErrors(List<HttpStatusCode> successStatusCodes, bool isSuccess, HttpStatusCode statusCode)
        {
            return !isSuccess;
        }
    }
}