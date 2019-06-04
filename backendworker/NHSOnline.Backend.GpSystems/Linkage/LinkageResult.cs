using System.Net;
using NHSOnline.Backend.GpSystems.Im1Connection;
using NHSOnline.Backend.GpSystems.Linkage;
using NHSOnline.Backend.GpSystems.Linkage.Models;

namespace NHSOnline.Backend.GpSystems.Linkage
{
    public abstract class LinkageResult
    {
        public abstract T Accept<T>(ILinkageResultVisitor<T> visitor);

        public class ErrorCase : LinkageResult
        {
            public Im1ConnectionErrorCodes.Code ErrorCode { get; }

            public ErrorCase(Im1ConnectionErrorCodes.Code errorCode)
            {
                ErrorCode = errorCode;
            }

            public override T Accept<T>(ILinkageResultVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }
        
        public class SupplierSystemUnavailable : LinkageResult
        {
            public override T Accept<T>(ILinkageResultVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }

        public class InternalServerError : LinkageResult
        {
            public override T Accept<T>(ILinkageResultVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }

        public class SuccessfullyCreated : LinkageResult
        {
            public LinkageResponse Response { get; }

            public SuccessfullyCreated(LinkageResponse response)
            {
                Response = response;
            }

            public override T Accept<T>(ILinkageResultVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }
        
        public class SuccessfullyRetrieved : LinkageResult
        {
            public LinkageResponse Response { get; }

            public SuccessfullyRetrieved(LinkageResponse response)
            {
                Response = response;
            }

            public override T Accept<T>(ILinkageResultVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }

        public class SuccessfullyRetrievedAlreadyExists : LinkageResult
        {
            public LinkageResponse Response { get; }

            public SuccessfullyRetrievedAlreadyExists(LinkageResponse response)
            {
                Response = response;
            }

            public override T Accept<T>(ILinkageResultVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }

        public class NotFound : LinkageResult
        {
            public Im1ConnectionErrorCodes.Code ErrorCode { get; }

            public NotFound(Im1ConnectionErrorCodes.Code errorCode)
            {
                ErrorCode = errorCode;
            }

            public override T Accept<T>(ILinkageResultVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }

        public class Forbidden : LinkageResult
        {
            public Im1ConnectionErrorCodes.Code ErrorCode { get; }

            public Forbidden(Im1ConnectionErrorCodes.Code errorCode)
            {
                ErrorCode = errorCode;
            }

            public override T Accept<T>(ILinkageResultVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }

        public class Conflict : LinkageResult
        {
            public Im1ConnectionErrorCodes.Code ErrorCode { get; }

            public Conflict(Im1ConnectionErrorCodes.Code errorCode)
            {
                ErrorCode = errorCode;
            }

            public override T Accept<T>(ILinkageResultVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }

        public class BadRequest : LinkageResult
        {
            public Im1ConnectionErrorCodes.Code ErrorCode { get; }

            public BadRequest(Im1ConnectionErrorCodes.Code errorCode)
            {
                ErrorCode = errorCode;
            }

            public override T Accept<T>(ILinkageResultVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }

        public class UnknownError : LinkageResult
        {
            public Im1ConnectionErrorCodes.Code ErrorCode { get; }

            public UnknownError(Im1ConnectionErrorCodes.Code errorCode)
            {
                ErrorCode = errorCode;
            }

            public override T Accept<T>(ILinkageResultVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }
    }
}
