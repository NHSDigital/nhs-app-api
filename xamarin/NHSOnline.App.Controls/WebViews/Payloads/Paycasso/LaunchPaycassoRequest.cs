namespace NHSOnline.App.Controls.WebViews.Payloads.Paycasso
{
    public sealed class LaunchPaycassoRequest
    {
        public PaycassoCredentials Credentials { get; set; } = new PaycassoCredentials();
        public PaycassoExternalReferences ExternalReferences { get; set; } = new PaycassoExternalReferences();
        public PaycassoTransactionDetails TransactionDetails { get; set; } = new PaycassoTransactionDetails();
    }
}