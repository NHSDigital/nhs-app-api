using System;
using NHSOnline.App.Areas.LoggedOut.Models;
using NHSOnline.App.Navigation;
using NHSOnline.App.NhsLogin;

namespace NHSOnline.App.Areas.WebIntegration.Models
{
    internal sealed class NhsLoginUpliftModel
    {
        internal NhsLoginUpliftModel(
            ProofKeyCodeExchangeCodes pkceCodes,
            string assertedLoginIdentity,
            INhsAppNavigationHandler navigationHandler)
        {
            PkceCodes = pkceCodes;
            AssertedLoginIdentity = assertedLoginIdentity;
            NavigationHandler = navigationHandler;
        }

        internal ProofKeyCodeExchangeCodes PkceCodes { get; }
        internal string AssertedLoginIdentity { get; }
        internal INhsAppNavigationHandler NavigationHandler { get; }

        internal CreateSessionModel AuthReturn(Uri redirectUri, string authCode) =>
            new CreateSessionModel(
                new NhsLoginModel(this.PkceCodes),
                redirectUri,
                authCode,
                null);

        internal NhsLoginErrorModel NhsLoginFailed() =>
            new NhsLoginErrorModel(new NhsLoginModel(this.PkceCodes),
                NhsLoginModel.GenerateErrorReferenceCodeFromPrefix());
    }
}