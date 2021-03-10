namespace NHSOnline.App.DependencyServices.Paycasso
{
    public sealed class PaycassoErrorResponse
    {
        public PaycassoError Error { get; } = new PaycassoError();
    }
}