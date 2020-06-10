using System.Net;

namespace NHSOnline.Backend.GpSystems.Suppliers.Emis.Client
{
    internal interface IEmisRequestBuilder
    {
        IEmisRequestBuilder SessionId(string sessionId);
        IEmisRequestBuilder EndUserSessionId(string endUserSessionId);
        IEmisRequestBuilder Timeout(int timeout);
        IEmisRequestBuilder AdditionalSuccessHttpStatusCode(HttpStatusCode successCode);
    }
}