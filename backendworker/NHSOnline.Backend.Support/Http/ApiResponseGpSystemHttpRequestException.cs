using System;

namespace NHSOnline.Backend.Support.Http
{
    public class ApiResponseGpSystemHttpRequestException : UnauthorisedGpSystemHttpRequestException
    {
        public ApiResponse ApiResponse { get; }

        public ApiResponseGpSystemHttpRequestException(string message) : base(message)
        {
        }

        public ApiResponseGpSystemHttpRequestException(string message, Exception innerException) : base(message, innerException)
        {
        }

        public ApiResponseGpSystemHttpRequestException(ApiResponse response)
        {
            ApiResponse = response;
        }

        public ApiResponseGpSystemHttpRequestException()
        {
        }
    }
}
