using System.Threading.Tasks;

namespace NHSOnline.App.DependencyServices.Paycasso
{
    public abstract class PaycassoResult
    {
        private PaycassoResult()
        { }

        public abstract Task Accept(IPaycassoResultVisitor visitor);

        public sealed class Success : PaycassoResult
        {
            public Success(string transactionId, PaycassoTransactionType transactionType, bool isFaceMatched)
            {
                Response = new PaycassoResponse
                {
                    TransactionId = transactionId,
                    TransactionType = transactionType,
                    IsFaceMatched = isFaceMatched
                };
            }

            public PaycassoResponse Response { get; }

            public override Task Accept(IPaycassoResultVisitor visitor) => visitor.Visit(this);
        }

        public sealed class Failure : PaycassoResult
        {
            public Failure(string errorMessage, int? errorCode = null)
            {
                Response = new PaycassoError
                {
                    ErrorMessage = errorMessage,
                    ErrorCode = errorCode
                };
            }

            public PaycassoError Response { get; }

            public override Task Accept(IPaycassoResultVisitor visitor) => visitor.Visit(this);
        }
    }
}