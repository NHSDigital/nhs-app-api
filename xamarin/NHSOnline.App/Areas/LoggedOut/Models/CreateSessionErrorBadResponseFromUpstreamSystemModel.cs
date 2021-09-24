namespace NHSOnline.App.Areas.LoggedOut.Models
{
    internal sealed class CreateSessionErrorBadResponseFromUpstreamSystemModel : CreateSessionModel
    {
        public CreateSessionErrorBadResponseFromUpstreamSystemModel(CreateSessionModel createSessionModel, string serviceDeskReference)
            : base(createSessionModel)
        {
            ServiceDeskReference = serviceDeskReference;
        }

        public string ServiceDeskReference { get; }
    }
}