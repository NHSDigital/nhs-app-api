using System.Collections.Generic;
using System.Linq;

namespace NHSOnline.Backend.Support
{
    public class ErrorCodes<TCode, TResponse> where TCode : struct where TResponse : IApiErrorResponse, new()
    {
        public Dictionary<int, TResponse> GetAllErrorResponses()
        {
            var all = EnumHelper.GetValues<TCode>();
            return all.ToDictionary(error => (int)(object)error, ToErrorResponse);
        }

        private static TResponse ToErrorResponse(TCode errorCode)
        {
            return new TResponse
            {
                ErrorCode = (int)(object)errorCode,
                ErrorMessage = EnumHelper.GetDescriptionOrThrowException(errorCode)
            };
        }
    }
}