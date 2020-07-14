namespace NHSOnline.App.Api.Client.Errors
{
    internal sealed class PfsErrorResponse
    {
        public PfsErrorResponse(string serviceDeskReference) => ServiceDeskReference = serviceDeskReference;

        public string ServiceDeskReference { get; }
    }
}
