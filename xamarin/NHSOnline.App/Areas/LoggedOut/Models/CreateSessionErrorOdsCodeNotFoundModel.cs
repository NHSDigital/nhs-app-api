namespace NHSOnline.App.Areas.LoggedOut.Models
{
    internal sealed class CreateSessionErrorOdsCodeNotFoundModel : CreateSessionModel
    {
        public CreateSessionErrorOdsCodeNotFoundModel(CreateSessionModel createSessionModel, string serviceDeskReference)
            : base(createSessionModel)
        {
            ServiceDeskReference = serviceDeskReference;
        }

        public string ServiceDeskReference { get; }
    }
}