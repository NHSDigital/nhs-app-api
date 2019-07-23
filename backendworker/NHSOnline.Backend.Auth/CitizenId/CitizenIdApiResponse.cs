using System.Net;
using NHSOnline.Backend.Auth.CitizenId.Models;

namespace NHSOnline.Backend.Auth.CitizenId
{
    public class CitizenIdApiResponse
    {
        public HttpStatusCode StatusCode { get; }
        
        public ErrorResponse ErrorResponse { get; set; }
        
        public bool HasSuccessStatusCode => (int) StatusCode >= 200 && (int) StatusCode <= 299;
        
        protected CitizenIdApiResponse(HttpStatusCode statusCode)
        {
            StatusCode = statusCode;
        }
    }
}