using NHSOnline.Backend.GpSystems.Messages.Models;

namespace NHSOnline.Backend.GpSystems.Messages
{
    public abstract class GetPatientMessagesResult
    {
        public abstract T Accept<T>(IPatientMessagesResultVisitor<T> visitor);
        
        public class Success : GetPatientMessagesResult
        {
            public GetPatientMessagesResponse Response { get; }

            public Success(GetPatientMessagesResponse response)
            {
                Response = response;
            }

            public override T Accept<T>(IPatientMessagesResultVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }

        public class BadGateway : GetPatientMessagesResult
        {
            public override T Accept<T>(IPatientMessagesResultVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }
        
        public class Forbidden : GetPatientMessagesResult
        {
            public override T Accept<T>(IPatientMessagesResultVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }
        
        public class BadRequest : GetPatientMessagesResult
        {
            public override T Accept<T>(IPatientMessagesResultVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }
        
        public class InternalServerError : GetPatientMessagesResult
        {
            public override T Accept<T>(IPatientMessagesResultVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }
    }
}