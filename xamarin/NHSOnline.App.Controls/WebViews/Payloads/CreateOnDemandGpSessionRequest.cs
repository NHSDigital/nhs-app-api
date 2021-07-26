namespace NHSOnline.App.Controls.WebViews.Payloads
{
    public class CreateOnDemandGpSessionRequest
    {
        public string AssertedLoginIdentity { get; set; }
        public string RedirectTo { get; set; }

        public CreateOnDemandGpSessionRequest(string assertedLoginIdentity, string redirectTo)
        {
            AssertedLoginIdentity = assertedLoginIdentity;
            RedirectTo = redirectTo;
        }
    }
}