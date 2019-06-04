using NHSOnline.Backend.GpSystems.Im1Connection.Models;

namespace NHSOnline.Backend.GpSystems.Im1Connection
{
    public abstract class Im1ConnectionRegisterResult
    {
        private Im1ConnectionRegisterResult()
        {
        }

        public abstract T Accept<T>(IIm1ConnectionRegisterResultVisitor<T> visitor);

        public class Success : Im1ConnectionRegisterResult
        {
            public PatientIm1ConnectionResponse Response { get; }

            public Success(PatientIm1ConnectionResponse response)
            {
                Response = response;
            }

            public override T Accept<T>(IIm1ConnectionRegisterResultVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }

        public class NotFound : Im1ConnectionRegisterResult
        {
            public Im1ConnectionErrorCodes.Code ErrorCode { get; }

            public NotFound(Im1ConnectionErrorCodes.Code errorCode)
            {
                ErrorCode = errorCode;
            }

            public override T Accept<T>(IIm1ConnectionRegisterResultVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }

        public class BadRequest : Im1ConnectionRegisterResult
        {
            public Im1ConnectionErrorCodes.Code ErrorCode { get; }

            public BadRequest(Im1ConnectionErrorCodes.Code errorCode)
            {
                ErrorCode = errorCode;
            }

            public override T Accept<T>(IIm1ConnectionRegisterResultVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }

        public class Conflict : Im1ConnectionRegisterResult
        {
            public Im1ConnectionErrorCodes.Code ErrorCode { get; }

            public Conflict(Im1ConnectionErrorCodes.Code errorCode)
            {
                ErrorCode = errorCode;
            }

            public override T Accept<T>(IIm1ConnectionRegisterResultVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }

        public class BadGateway : Im1ConnectionRegisterResult
        {
            public override T Accept<T>(IIm1ConnectionRegisterResultVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }

        public class UnknownError : Im1ConnectionRegisterResult
        {
            public Im1ConnectionErrorCodes.Code ErrorCode { get; }

            public UnknownError(Im1ConnectionErrorCodes.Code errorCode)
            {
                ErrorCode = errorCode;
            }

            public override T Accept<T>(IIm1ConnectionRegisterResultVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }

        public class ErrorCase : Im1ConnectionRegisterResult
        {
            public Im1ConnectionErrorCodes.Code ErrorCode { get; }

            public ErrorCase(Im1ConnectionErrorCodes.Code errorCode)
            {
                ErrorCode = errorCode;
            }

            public override T Accept<T>(IIm1ConnectionRegisterResultVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }
    }
}