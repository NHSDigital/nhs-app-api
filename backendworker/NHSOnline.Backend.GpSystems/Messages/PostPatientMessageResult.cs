using NHSOnline.Backend.GpSystems.Messages.Models;

namespace NHSOnline.Backend.GpSystems.Messages
{
    public abstract class PostPatientMessageResult
    {
        public abstract T Accept<T>(IPatientSendMessageResultVisitor<T> visitor);
        
        public class Success : PostPatientMessageResult
        {
            public PostPatientMessageResponse Response { get; }

            public Success(PostPatientMessageResponse response)
            {
                Response = response;
            }

            public override T Accept<T>(IPatientSendMessageResultVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }

        public class BadGateway : PostPatientMessageResult
        {
            public override T Accept<T>(IPatientSendMessageResultVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }
        
        public class Forbidden : PostPatientMessageResult
        {
            public override T Accept<T>(IPatientSendMessageResultVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }
        
        public class BadRequest : PostPatientMessageResult
        {
            public override T Accept<T>(IPatientSendMessageResultVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }
        
        public class InternalServerError : PostPatientMessageResult
        {
            public override T Accept<T>(IPatientSendMessageResultVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }
    }
}