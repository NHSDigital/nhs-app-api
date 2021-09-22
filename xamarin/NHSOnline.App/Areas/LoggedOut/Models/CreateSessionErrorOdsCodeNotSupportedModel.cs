namespace NHSOnline.App.Areas.LoggedOut.Models
{
    internal sealed class CreateSessionErrorOdsCodeNotSupportedModel : CreateSessionModel
    {
        public CreateSessionErrorOdsCodeNotSupportedModel(CreateSessionModel createSessionModel, string serviceDeskReference)
            : base(createSessionModel)
        {
            ServiceDeskReference = serviceDeskReference;
        }

        public string ServiceDeskReference { get; }
    }
}