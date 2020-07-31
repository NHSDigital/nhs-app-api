using System;
using NHSOnline.App.NhsLogin;

namespace NHSOnline.App.Areas.LoggedOut.Models
{
    internal class NhsLoginModel
    {
        internal NhsLoginModel(ProofKeyCodeExchangeCodes pkceCodes)
        {
            PkceCodes = pkceCodes;
        }

        protected NhsLoginModel(NhsLoginModel nhsLoginModel) : this(nhsLoginModel.PkceCodes)
        {
        }

        internal ProofKeyCodeExchangeCodes PkceCodes { get; }

        internal CreateSessionModel AuthReturn(Uri redirectUri, string authCode) => new CreateSessionModel(this, redirectUri, authCode);

        internal NhsLoginErrorModel NhsLoginFailed(string errorReferenceCode) => new NhsLoginErrorModel(this, errorReferenceCode);
    }
}