using NHSOnline.Backend.Worker.GpSystems.Im1Connection.Models;

namespace NHSOnline.Backend.Worker.GpSystems.Im1Connection
{
    public abstract class Im1ConnectionVerifyResult
    {
        private Im1ConnectionVerifyResult()
        {
        }

        public abstract T Accept<T>(IIm1ConnectionVerifyResultVisitor<T> visitor);

        public class SuccessfullyVerified : Im1ConnectionVerifyResult
        {
            public PatientIm1ConnectionResponse Response { get; }

            public SuccessfullyVerified(PatientIm1ConnectionResponse response)
            {
                Response = response;
            }

            public override T Accept<T>(IIm1ConnectionVerifyResultVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }

        public class InsufficientPermissions : Im1ConnectionVerifyResult
        {
            public override T Accept<T>(IIm1ConnectionVerifyResultVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }

        public class NotFound : Im1ConnectionVerifyResult
        {
            public override T Accept<T>(IIm1ConnectionVerifyResultVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }

        public class SupplierSystemUnavailable : Im1ConnectionVerifyResult
        {
            public override T Accept<T>(IIm1ConnectionVerifyResultVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }

        public class InvalidRequest : Im1ConnectionVerifyResult
        {
            public override T Accept<T>(IIm1ConnectionVerifyResultVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }

        public class ErrorProcessingSecurityHeader : Im1ConnectionVerifyResult
        {
            public override T Accept<T>(IIm1ConnectionVerifyResultVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }

        public class InvalidUserCredentials : Im1ConnectionVerifyResult
        {
            public override T Accept<T>(IIm1ConnectionVerifyResultVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }

        public class UnknownError : Im1ConnectionVerifyResult
        {
            public override T Accept<T>(IIm1ConnectionVerifyResultVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }
    }
}