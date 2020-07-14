namespace NHSOnline.App.Areas.LoggedOut.Models
{
    internal sealed class CreateSessionErrorFallbackModel : CreateSessionModel
    {
        public CreateSessionErrorFallbackModel(CreateSessionModel createSessionModel)
            : base(createSessionModel)
        {
        }
    }
}