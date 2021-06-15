namespace NHSOnline.App.Areas.LoggedOut.Models
{
    internal sealed class CreateSessionErrorBadRequestModel : CreateSessionModel
    {
        public CreateSessionErrorBadRequestModel(CreateSessionModel createSessionModel, string serviceDeskReference)
            : base(createSessionModel)
        {
            ServiceDeskReference = serviceDeskReference;
            AccessibleServiceDeskReference = string.Join(" ", serviceDeskReference.ToCharArray());
        }

        public string ServiceDeskReference { get; }
        public string AccessibleServiceDeskReference { get; }
    }
}