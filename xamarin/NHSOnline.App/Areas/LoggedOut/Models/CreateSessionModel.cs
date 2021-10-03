using System;

namespace NHSOnline.App.Areas.LoggedOut.Models
{
    internal class CreateSessionModel: NhsLoginModel
    {
        public CreateSessionModel(NhsLoginModel nhsLoginModel, Uri redirectUri, string authCode, Uri? deeplinkUrl)
            : base(nhsLoginModel.PkceCodes, nhsLoginModel.FidoAuthResponse, deeplinkUrl)
        {
            RedirectUri = redirectUri;
            AuthCode = authCode;
        }

        protected CreateSessionModel(CreateSessionModel createSessionModel)
            : this(createSessionModel, createSessionModel.RedirectUri, createSessionModel.AuthCode, createSessionModel.DeeplinkUrl)
        {
        }

        public Uri RedirectUri { get; }
        public string AuthCode { get; }

        internal CreateSessionErrorInternalServerErrorModel InternalServerError(string serviceDeskReference)
            => new CreateSessionErrorInternalServerErrorModel(this, serviceDeskReference);

        internal CreateSessionErrorFallbackModel FallbackError()
            => new CreateSessionErrorFallbackModel(this);

        public CreateSessionErrorBadRequestModel BadRequestError(string serviceDeskReference)
            => new CreateSessionErrorBadRequestModel(this, serviceDeskReference);

        public CreateSessionErrorOdsCodeNotSupportedOrNoNhsNumberModel OdsCodeNotSupportedOrNoNhsNumberError(string serviceDeskReference)
            => new CreateSessionErrorOdsCodeNotSupportedOrNoNhsNumberModel(this, serviceDeskReference);

        public CreateSessionErrorFailedAgeRequirementModel FailedAgeRequirementError()
            => new CreateSessionErrorFailedAgeRequirementModel(this);

        public CreateSessionErrorBadResponseFromUpstreamSystemModel BadResponseFromUpstreamSystemError(string serviceDeskReference)
            => new CreateSessionErrorBadResponseFromUpstreamSystemModel(this, serviceDeskReference);

        public CreateSessionErrorUpstreamSystemTimeoutModel UpstreamSystemTimeoutError(string serviceDeskReference)
            => new CreateSessionErrorUpstreamSystemTimeoutModel(this, serviceDeskReference);
    }
}
