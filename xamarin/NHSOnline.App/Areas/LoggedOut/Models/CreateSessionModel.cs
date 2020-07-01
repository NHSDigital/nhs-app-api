using System;

namespace NHSOnline.App.Areas.LoggedOut.Models
{
    internal class CreateSessionModel: NhsLoginModel
    {
        public CreateSessionModel(NhsLoginModel nhsLoginModel, Uri redirectUri, string authCode)
            : base(nhsLoginModel)
        {
            RedirectUri = redirectUri;
            AuthCode = authCode;
        }

        protected CreateSessionModel(CreateSessionModel createSessionModel)
            : this(createSessionModel, createSessionModel.RedirectUri, createSessionModel.AuthCode)
        {
        }

        public Uri RedirectUri { get; }
        public string AuthCode { get; }

        internal CreateSessionErrorModel CreateSessionFailed() => new CreateSessionErrorModel(this);
    }
}
