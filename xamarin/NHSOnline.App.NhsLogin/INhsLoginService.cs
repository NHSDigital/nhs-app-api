namespace NHSOnline.App.NhsLogin
{
    public interface INhsLoginService
    {
        ProofKeyCodeExchangeCodes GeneratePkceCodes();
        LoginState BeginLogin(ProofKeyCodeExchangeCodes codes);
    }
}