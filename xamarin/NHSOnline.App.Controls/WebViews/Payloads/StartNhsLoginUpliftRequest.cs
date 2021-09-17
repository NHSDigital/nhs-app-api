namespace NHSOnline.App.Controls.WebViews.Payloads
{
    public sealed class StartNhsLoginUpliftRequest
    {
        public StartNhsLoginUpliftRequest(string assertedLoginIdentity)
        {
            AssertedLoginIdentity = assertedLoginIdentity;
        }

        public string AssertedLoginIdentity { get; }
    }
}