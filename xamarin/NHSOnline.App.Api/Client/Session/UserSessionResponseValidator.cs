namespace NHSOnline.App.Api.Client.Session
{
    internal sealed class UserSessionResponseValidator :
        IResponseModelValidator<UserSessionResponseModel, UserSessionResponse>
    {
        public UserSessionResponse Validate(UserSessionResponseModel model)
        {
            return new UserSessionResponse(
                model.Name,
                model.SessionTimeout,
                model.OdsCode,
                model.Token,
                model.NhsNumber,
                model.DateOfBirth,
                model.AccessToken,
                model.Im1MessagingEnabled,
                model.ProofLevel);
        }
    }
}