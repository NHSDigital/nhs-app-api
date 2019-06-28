using NHSOnline.Backend.Support;

namespace NHSOnline.Backend.GpSystems.Im1Connection.Models
{
    public class Im1ErrorResponse : ApiErrorResponse
    {
        public string GpSystem { get; set; }
    }
}