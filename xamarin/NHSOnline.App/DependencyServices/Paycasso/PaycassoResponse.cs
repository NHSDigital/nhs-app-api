namespace NHSOnline.App.DependencyServices.Paycasso
{
    public sealed class PaycassoResponse
    {
        public string TransactionId { get; set; } = string.Empty;
        public PaycassoTransactionType TransactionType { get; set; }
        public bool IsFaceMatched { get; set; }
    }
}