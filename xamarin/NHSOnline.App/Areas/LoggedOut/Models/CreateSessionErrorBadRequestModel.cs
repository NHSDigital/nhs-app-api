namespace NHSOnline.App.Areas.LoggedOut.Models
{
    internal sealed class CreateSessionErrorBadRequestModel : CreateSessionModel
    {
        public CreateSessionErrorBadRequestModel(CreateSessionModel createSessionModel, string serviceDeskReference)
            : base(createSessionModel)
        {
            ServiceDeskReference = serviceDeskReference;
        }

        public string ServiceDeskReference { get; }
    }
}