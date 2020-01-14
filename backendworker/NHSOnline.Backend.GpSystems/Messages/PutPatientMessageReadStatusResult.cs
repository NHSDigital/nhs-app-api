using NHSOnline.Backend.GpSystems.Messages.Models;

namespace NHSOnline.Backend.GpSystems.Messages
{
    public abstract class PutPatientMessageReadStatusResult
    {
        public abstract T Accept<T>(IPatientMessageUpdateReadStatusResultVisitor<T> visitor);
        
        public class Success : PutPatientMessageReadStatusResult
        {
            public PutPatientMessageUpdateStatusResponse Response { get; }

            public Success(PutPatientMessageUpdateStatusResponse response)
            {
                Response = response;
            }

            public override T Accept<T>(IPatientMessageUpdateReadStatusResultVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }

        public class BadGateway : PutPatientMessageReadStatusResult
        {
            public override T Accept<T>(IPatientMessageUpdateReadStatusResultVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }
        
        public class Forbidden : PutPatientMessageReadStatusResult
        {
            public override T Accept<T>(IPatientMessageUpdateReadStatusResultVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }
        
        public class BadRequest : PutPatientMessageReadStatusResult
        {
            public override T Accept<T>(IPatientMessageUpdateReadStatusResultVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }
        
        public class InternalServerError : PutPatientMessageReadStatusResult
        {
            public override T Accept<T>(IPatientMessageUpdateReadStatusResultVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }
    }
}