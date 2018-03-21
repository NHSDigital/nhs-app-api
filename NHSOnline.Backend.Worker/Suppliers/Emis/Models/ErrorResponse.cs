namespace NHSOnline.Backend.Worker.Suppliers.Emis.Models
{
    public class ErrorResponse
    {
        public ErrorResponseException[] Exceptions { get; set; }
        public int InternalResponseCode { get; set; }
        public string InternalResponseMethod { get; set; }
    }
}