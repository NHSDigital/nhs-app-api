namespace NHSOnline.App.Areas.LoggedOut.Models
{
    internal sealed class CreateSessionErrorOdsCodeNotSupportedOrNoNhsNumberModel : CreateSessionModel
    {
        public CreateSessionErrorOdsCodeNotSupportedOrNoNhsNumberModel(CreateSessionModel createSessionModel, string serviceDeskReference)
            : base(createSessionModel)
        {
            ServiceDeskReference = serviceDeskReference;
            AccessibleServiceDeskReference = string.Join(" ", serviceDeskReference.ToCharArray());
        }

        public string AccessibleServiceDeskReference { get; set; }

        public string ServiceDeskReference { get; }
    }
}