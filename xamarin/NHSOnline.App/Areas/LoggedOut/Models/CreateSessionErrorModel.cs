namespace NHSOnline.App.Areas.LoggedOut.Models
{
    internal sealed class CreateSessionErrorModel : CreateSessionModel
    {
        public CreateSessionErrorModel(CreateSessionModel createSessionModel)
            : base(createSessionModel)
        {
        }
    }
}