using NHSOnline.Backend.GpSystems.Messages.Models;

namespace NHSOnline.Backend.GpSystems.Messages
{
    public abstract class GetPatientMessageResult
    {
        public abstract T Accept<T>(IPatientMessageResultVisitor<T> visitor);
        
        public class Success : GetPatientMessageResult
        {
            public GetPatientMessageResponse Response { get; }

            public Success(GetPatientMessageResponse response)
            {
                Response = response;
            }

            public override T Accept<T>(IPatientMessageResultVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }

        public class BadGateway : GetPatientMessageResult
        {
            public override T Accept<T>(IPatientMessageResultVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }
        
        public class Forbidden : GetPatientMessageResult
        {
            public override T Accept<T>(IPatientMessageResultVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }
        
        public class BadRequest : GetPatientMessageResult
        {
            public override T Accept<T>(IPatientMessageResultVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }
        
        public class InternalServerError : GetPatientMessageResult
        {
            public override T Accept<T>(IPatientMessageResultVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }
    }
}