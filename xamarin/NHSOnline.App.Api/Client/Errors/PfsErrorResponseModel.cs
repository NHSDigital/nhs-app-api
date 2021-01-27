namespace NHSOnline.App.Api.Client.Errors
{
    internal sealed class PfsErrorResponseModel
    {
        public int? ErrorCode { get; set; }
        public string? ErrorMessage { get; set; }
        public string? ServiceDeskReference { get; set; }
    }
}