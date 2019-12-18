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
        T ParseBody<T>(string stringResponse);
        T ParseError<T>(string stringResponse,
            HttpResponseMessage message,
            params HttpStatusCode[] allowedErrorStatuses);
    }
}