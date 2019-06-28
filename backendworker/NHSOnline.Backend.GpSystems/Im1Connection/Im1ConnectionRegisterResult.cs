using System.Net;
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

        public class BadGateway : Im1ConnectionRegisterResult
        {
            public override T Accept<T>(IIm1ConnectionRegisterResultVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }

        public class UnmappedErrorWithStatusCode : Im1ConnectionRegisterResult
        {
            public Im1ConnectionErrorCodes.InternalCode ErrorCode { get; }

            public UnmappedErrorWithStatusCode()
            {
                ErrorCode = Im1ConnectionErrorCodes.InternalCode.UnknownError;
            }

            public override T Accept<T>(IIm1ConnectionRegisterResultVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }

        public class ErrorCase : Im1ConnectionRegisterResult
        {
            public Im1ConnectionErrorCodes.InternalCode ErrorCode { get; }

            public ErrorCase(Im1ConnectionErrorCodes.InternalCode errorCode)
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