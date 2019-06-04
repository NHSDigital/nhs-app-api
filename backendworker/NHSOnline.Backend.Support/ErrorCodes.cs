using System;
using System.Collections.Generic;
using System.Linq;

namespace NHSOnline.Backend.Support
{
    public class ErrorCodes<T> where T : struct, IConvertible
    {
        public Dictionary<int, ApiErrorResponse> GetAllErrorResponses()
        {
            var all = (T[]) Enum.GetValues(typeof(T));
            return all.ToDictionary(error => (int) (object) error, ToErrorResponse);
        }
        
        private static ApiErrorResponse ToErrorResponse(T errorCode)
        {
            return new ApiErrorResponse
            {
                ErrorCode = (int)(object)errorCode,
                ErrorMessage = EnumHelper.GetDescriptionOrThrowException(errorCode)
            };
        }
    }
}