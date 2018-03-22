namespace NHSOnline.Backend.Worker.Bridges.Emis.Models
{
    public class ErrorResponse
    {
        public ErrorResponseException[] Exceptions { get; set; }
        public int InternalResponseCode { get; set; }
        public string InternalResponseMethod { get; set; }
    }
}