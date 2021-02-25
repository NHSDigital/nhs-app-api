namespace NHSOnline.App.iOS.DependencyServices
{
    public sealed class PaycassoCallbackResponse
    {
        private PaycassoCallbackResponse()
        { }

        public string? TransactionId { get; set; }
        public PaycassoTransactionType? TransactionType { get; set; }
        public bool? IsFaceMatched { get; set; } = false;
        public PaycassoError? Error { get; set; }

        public static PaycassoCallbackResponse ForSuccess(
            string transactionId,
            PaycassoTransactionType transactionType,
            bool isFaceMatched,
            PaycassoError? error)
        {
            return new PaycassoCallbackResponse
            {
                TransactionId = transactionId,
                TransactionType = transactionType,
                IsFaceMatched = isFaceMatched,
                Error = error
            };
        }

        public static PaycassoCallbackResponse ForError(string errorMessage, int? errorCode = null)
        {
            return new PaycassoCallbackResponse
            {
                Error = new PaycassoError(errorMessage)
                {
                    ErrorCode = errorCode
                }
            };
        }
    }
}