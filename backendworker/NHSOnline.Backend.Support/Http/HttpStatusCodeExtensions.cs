using System.Net;

namespace NHSOnline.Backend.Support.Http
{
    public static class HttpStatusCodeExtensions
    {
        public static bool IsSuccessStatusCode(this HttpStatusCode statusCode)
        {
            return IsSuccessStatusCode((int) statusCode);
        }

        public static bool IsBadRequestCode(this HttpStatusCode statusCode)
        {
            return (int) statusCode >= 400 && (int) statusCode <= 499 ;
        }
        
        public static bool IsSuccessStatusCode(this int statusCode)
        {
            return statusCode >= 200 && statusCode <= 299;
        }
    }
}
