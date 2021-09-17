namespace NHSOnline.App.NhsLogin
{
    public interface INhsLoginService
    {
        ProofKeyCodeExchangeCodes GeneratePkceCodes();
        LoginState BeginLogin(ProofKeyCodeExchangeCodes codes, string? fidoAuthResponse);
        CreateOnDemandGpSessionState CreateOnDemandGpSession(string assertedLoginIdentity, string redirectTo);
        LoginState CreateNhsLoginUpliftSession(ProofKeyCodeExchangeCodes codes, string assertedLoginIdentity);
    }
}