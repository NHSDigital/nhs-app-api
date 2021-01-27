namespace NHSOnline.App.Areas.LoggedOut.Models
{
    internal sealed class CreateSessionErrorOdsCodeNotSupportedOrNoNhsNumberModel : CreateSessionModel
    {
        public CreateSessionErrorOdsCodeNotSupportedOrNoNhsNumberModel(CreateSessionModel createSessionModel, string serviceDeskReference)
            : base(createSessionModel)
        {
            ServiceDeskReference = serviceDeskReference;
        }

        public string ServiceDeskReference { get; }
    }
}