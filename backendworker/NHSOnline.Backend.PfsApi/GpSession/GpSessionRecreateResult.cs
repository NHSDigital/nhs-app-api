using NHSOnline.Backend.Auditing;
using NHSOnline.Backend.Support;

namespace NHSOnline.Backend.PfsApi.GpSession
{
    public abstract class GpSessionRecreateResult : IAuditedResult
    {
        public abstract string Details { get; }

        public abstract T Accept<T>(IGpSessionRecreateResultVisitor<T> supplierVisitor);

        public class RecreatedResult : GpSessionRecreateResult
        {
            public override string Details => "GP Session Recreated";

            public override T Accept<T>(IGpSessionRecreateResultVisitor<T> supplierVisitor)
            {
                return supplierVisitor.Visit(this);
            }
        }

        public class SessionStillValidResult : GpSessionRecreateResult
        {
            public override string Details => "GP Session is still valid - not recreating";

            public override T Accept<T>(IGpSessionRecreateResultVisitor<T> supplierVisitor)
            {
                return supplierVisitor.Visit(this);
            }
        }

        public class ErrorResult : GpSessionRecreateResult
        {
            public override string Details { get; }
            public ErrorTypes ErrorType { get; }

            public ErrorResult(ErrorTypes errorType, string details)
            {
                ErrorType = errorType;
                Details = details;
            }

            public override T Accept<T>(IGpSessionRecreateResultVisitor<T> supplierVisitor)
            {
                return supplierVisitor.Visit(this);
            }
        }
    }
}