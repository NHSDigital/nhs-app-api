namespace NHSOnline.Backend.Support
{
    public class PfsErrorResponse : IApiErrorResponse
    {
        public string ServiceDeskReference { get; set; }
        public int ErrorCode { get; set; }
        public string ErrorMessage { get; set; }
    }
}