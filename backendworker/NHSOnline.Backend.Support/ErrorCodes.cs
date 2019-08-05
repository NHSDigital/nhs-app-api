using System.Collections.Generic;
using System.Linq;

namespace NHSOnline.Backend.Support
{
    public class ErrorCodes<T, V> where T : struct where V : ApiErrorResponse, new()
    {
        public Dictionary<int, V> GetAllErrorResponses()
        {
            var all = EnumHelper.GetValues<T>();
            return all.ToDictionary(error => (int) (object) error, ToErrorResponse);
        }
        
        private static V ToErrorResponse(T errorCode)
        {
            return new V
            {
                ErrorCode = (int)(object)errorCode,
                ErrorMessage = EnumHelper.GetDescriptionOrThrowException(errorCode)
            };
        }
    }
}