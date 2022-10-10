using System;
using NHSOnline.App.ErrorReferences;
using NHSOnline.App.NhsLogin;

namespace NHSOnline.App.Areas.LoggedOut.Models
{
    internal class NhsLoginModel
    {
        internal NhsLoginModel(ProofKeyCodeExchangeCodes pkceCodes)
        {
            PkceCodes = pkceCodes;
            FidoAuthResponse = string.Empty;
            DeeplinkUrl = null;
        }

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

        internal CreateSessionModel AuthReturn(Uri redirectUri, string authCode, Uri? deeplinkUrl) =>
            new CreateSessionModel(this, redirectUri, authCode, deeplinkUrl);

        internal NhsLoginErrorModel NhsLoginFailed() =>
            new NhsLoginErrorModel(this, GenerateErrorReferenceCodeFromPrefix());

        internal static string GenerateErrorReferenceCodeFromPrefix() =>
            $"3w{RandomErrorReferenceGenerator.GenerateString(4, "acefghjkmnorstuwxyz3456789")}";
    }
}