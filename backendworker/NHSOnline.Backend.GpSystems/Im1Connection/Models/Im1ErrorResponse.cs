using NHSOnline.Backend.Support;

namespace NHSOnline.Backend.GpSystems.Im1Connection.Models
{
    public class Im1ErrorResponse : IApiErrorResponse
    {
        public int ErrorCode { get; set; }
        public string ErrorMessage { get; set; }
        public string GpSystem { get; set; }
    }
}