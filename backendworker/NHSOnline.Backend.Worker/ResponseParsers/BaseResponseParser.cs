using System.Linq;
using System.Net;
using System.Net.Http;

namespace NHSOnline.Backend.Worker.ResponseParsers
{
    public abstract class BaseResponseParser: IResponseParser
    {
        public T ParseBadRequest<T>(string stringResponse, HttpResponseMessage message)
        {
            return message.StatusCode != HttpStatusCode.BadRequest
                ? default(T)
                : Deserialize<T>(stringResponse);
        }
        
        public T ParseBody<T>(string stringResponse, HttpResponseMessage message)
        {
            return message.IsSuccessStatusCode ? Deserialize<T>(stringResponse) : default(T);
        }

        public T ParseError<T>(
            string stringResponse, 
            HttpResponseMessage message, 
            params HttpStatusCode[] allowedErrorStatuses)
        {
            
            return message.IsSuccessStatusCode || allowedErrorStatuses.Any(x => x == message.StatusCode)
                ? default(T)
                : Deserialize<T>(stringResponse);
        }

        protected abstract T Deserialize<T>(string body);
    }
}