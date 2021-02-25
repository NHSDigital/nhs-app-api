namespace NHSOnline.App.iOS.DependencyServices
{
    public sealed class PaycassoError
    {
        public int? ErrorCode { get; set; }
        public string ErrorMessage { get; }

        public PaycassoError(string errorMessage)
        {
            ErrorMessage = errorMessage;
        }
    }
}