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

        public class SessionNotRequiredResult : GpSessionRecreateResult
        {
            public override string Details => "GP Session is still not required - not creating";

            public override T Accept<T>(IGpSessionRecreateResultVisitor<T> supplierVisitor)
            {
                return supplierVisitor.Visit(this);
            }
        }

        public class Im1ConnectionTokenEmptyResult : GpSessionRecreateResult
        {
            public override string Details => "Im1Connection Token is empty. GP User Session is now required";

            public override T Accept<T>(IGpSessionRecreateResultVisitor<T> supplierVisitor)
            {
                return supplierVisitor.Visit(this);
            }
        }

        public class ErrorResult : GpSessionRecreateResult
        {
            public override string Details { get; }
            public ErrorTypes ErrorType { get; }

            public ErrorResult(ErrorTypes errorType, string details = "Gp session recreate failed")
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
