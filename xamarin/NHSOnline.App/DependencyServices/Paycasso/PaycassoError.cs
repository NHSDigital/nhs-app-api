namespace NHSOnline.App.DependencyServices.Paycasso
{
    public sealed class PaycassoError
    {
        public int? ErrorCode { get; set; }
        public string ErrorMessage { get; set; } = string.Empty;
    }
}