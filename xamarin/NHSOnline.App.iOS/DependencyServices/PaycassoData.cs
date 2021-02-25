namespace NHSOnline.App.iOS.DependencyServices
{
    public sealed class PaycassoData
    {
        public PaycassoCredentials Credentials { get; set; } = new PaycassoCredentials();
        public PaycassoExternalReferences ExternalReferences { get; set; } = new PaycassoExternalReferences();
        public PaycassoTransactionDetails TransactionDetails { get; set; } = new PaycassoTransactionDetails();
    }
}