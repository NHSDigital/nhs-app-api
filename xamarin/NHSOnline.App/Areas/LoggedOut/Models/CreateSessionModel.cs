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

        internal CreateSessionErrorModel Error(string serviceDeskReference)
            => new CreateSessionErrorModel(this, serviceDeskReference);

        internal CreateSessionErrorFallbackModel FallbackError()
            => new CreateSessionErrorFallbackModel(this);

        public CreateSessionErrorBadRequestModel BadRequestError(string serviceDeskReference)
            => new CreateSessionErrorBadRequestModel(this, serviceDeskReference);

        public CreateSessionErrorForbiddenModel ForbiddenError(string serviceDeskReference)
            => new CreateSessionErrorForbiddenModel(this, serviceDeskReference);
    }
}
