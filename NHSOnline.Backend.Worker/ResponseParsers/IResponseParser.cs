using System.Linq;
using System.Net;
using System.Net.Http;

namespace NHSOnline.Backend.Worker.ResponseParsers
{
    public interface IJsonResponseParser : IResponseParser
    {
    }

    public interface IXmlResponseParser : IResponseParser
    {
    }

    public interface IResponseParser
    {
        T ParseBadRequest<T>(string stringResponse, HttpResponseMessage message);
        T ParseBody<T>(string stringResponse, HttpResponseMessage message);
        T ParseError<T>(
            string stringResponse,
            HttpResponseMessage message,
            params HttpStatusCode[] allowedErrorStatuses);
    }
}