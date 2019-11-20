using System.Net;
using System.Net.Http;

namespace NHSOnline.Backend.Support.ResponseParsers
{
    public interface IJsonResponseParser : IResponseParser
    {
    }

    public interface IXmlResponseParser : IResponseParser
    {
    }

    public interface IResponseParser
    {
        bool TryParseBadRequest<T>(string stringResponse, HttpResponseMessage message, out T response);
        bool TryParseBody<T>(string stringResponse, HttpResponseMessage message, out T response);
        bool TryParseError<T>(string stringResponse,
            HttpResponseMessage message,
            out T response,
            params HttpStatusCode[] allowedErrorStatuses);
    }
}