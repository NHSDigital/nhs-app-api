using System.Linq;
using System.Net;
using System.Net.Http;
using NHSOnline.Backend.Support.AspNet.Filters;

namespace NHSOnline.Backend.Support.ResponseParsers
{
    public abstract class BaseResponseParser: IResponseParser
    {
        public bool TryParseBadRequest<T>(string stringResponse, HttpResponseMessage message, out T response)
        {
            try
            {
                if (message.StatusCode == HttpStatusCode.BadRequest)
                {
                    response = Deserialize<T>(stringResponse);
                    return response != null;
                }
                response = default;
                return false;
            }
            catch
            {
                response = default;
                throw new NhsUnparsableException($"Response parsing failed. Raw response: {stringResponse}");
            }
        }

        public bool TryParseBody<T>(string stringResponse, HttpResponseMessage message, out T response)
        {
            try
            {
                response = Deserialize<T>(stringResponse);
                return response != null;
            }
            catch
            {
                response = default;
                throw new NhsUnparsableException($"Response parsing failed. Raw response: {stringResponse}");
            }
        }

        public bool TryParseError<T>(string stringResponse,
            HttpResponseMessage message,
            out T response,
            params HttpStatusCode[] allowedErrorStatuses)
        {

            try
            {
                if (message.IsSuccessStatusCode || allowedErrorStatuses.Any(x => x == message.StatusCode))
                {
                    response = default;
                    return false;
                }

                response = Deserialize<T>(stringResponse);
                return response != null;
            }
            catch
            {
                response = default;
                throw new NhsUnparsableException($"Response parsing failed. Raw response: {stringResponse}");
            }
        }

        protected abstract T Deserialize<T>(string body);
    }
}