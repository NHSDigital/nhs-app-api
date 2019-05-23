using NHSOnline.Backend.GpSystems.Im1Connection.Models;

namespace NHSOnline.Backend.GpSystems.Im1Connection
{
    public abstract class Im1ConnectionVerifyResult
    {
        private Im1ConnectionVerifyResult()
        {
        }

        public abstract T Accept<T>(IIm1ConnectionVerifyResultVisitor<T> visitor);

        public class Success : Im1ConnectionVerifyResult
        {
            public PatientIm1ConnectionResponse Response { get; }

            public Success(PatientIm1ConnectionResponse response)
            {
                Response = response;
            }

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

        public class BadGateway : Im1ConnectionVerifyResult
        {
            public override T Accept<T>(IIm1ConnectionVerifyResultVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }

        public class BadRequest : Im1ConnectionVerifyResult
        {
            public override T Accept<T>(IIm1ConnectionVerifyResultVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }

        public class InternalServerError : Im1ConnectionVerifyResult
        {
            public override T Accept<T>(IIm1ConnectionVerifyResultVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }
    }
}