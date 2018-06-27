namespace NHSOnline.Backend.Worker.GpSystems.Suppliers.Emis
{
    public class ErrorResponse
    {
        public ErrorResponseException[] Exceptions { get; set; }
        public int InternalResponseCode { get; set; }
        public string InternalResponseMethod { get; set; }
    }
}