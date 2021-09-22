namespace NHSOnline.App.Areas.LoggedOut.Models
{
    internal sealed class CreateSessionErrorNoNhsNumberModel : CreateSessionModel
    {
        public CreateSessionErrorNoNhsNumberModel(CreateSessionModel createSessionModel, string serviceDeskReference)
            : base(createSessionModel)
        {
            ServiceDeskReference = serviceDeskReference;
        }

        public string ServiceDeskReference { get; }
    }
}