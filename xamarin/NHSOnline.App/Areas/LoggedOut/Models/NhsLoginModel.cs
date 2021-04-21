using System;
using NHSOnline.App.NhsLogin;

namespace NHSOnline.App.Areas.LoggedOut.Models
{
    internal class NhsLoginModel
    {
        internal NhsLoginModel(ProofKeyCodeExchangeCodes pkceCodes, string? fidoAuthResponse, Uri? deeplinkUrl)
        {
            PkceCodes = pkceCodes;
            FidoAuthResponse = fidoAuthResponse;
            DeeplinkUrl = deeplinkUrl;
        }

        protected NhsLoginModel(NhsLoginModel nhsLoginModel)
            : this(nhsLoginModel.PkceCodes, nhsLoginModel.FidoAuthResponse, nhsLoginModel.DeeplinkUrl)
        {
        }

        internal ProofKeyCodeExchangeCodes PkceCodes { get; }
        internal string? FidoAuthResponse { get; }

        internal Uri? DeeplinkUrl { get; }

        internal CreateSessionModel AuthReturn(Uri redirectUri, string authCode, Uri? deeplinkUrl) => new CreateSessionModel(this, redirectUri, authCode, deeplinkUrl);

        internal NhsLoginErrorModel NhsLoginFailed(string errorReferenceCode) => new NhsLoginErrorModel(this, errorReferenceCode);
    }
}