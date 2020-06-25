using System;

namespace NHSOnline.App.Areas.LoggedOut.Models
{
    internal sealed class CreateSessionModel: NhsLoginModel
    {
        public CreateSessionModel(NhsLoginModel nhsLoginModel, Uri redirectUri, string authCode)
            : base(nhsLoginModel)
        {
            RedirectUri = redirectUri;
            AuthCode = authCode;
        }

        public Uri RedirectUri { get; }
        public string AuthCode { get; }
    }
}
