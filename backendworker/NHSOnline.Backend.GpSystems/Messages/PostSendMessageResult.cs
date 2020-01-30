using NHSOnline.Backend.GpSystems.Messages.Models;

namespace NHSOnline.Backend.GpSystems.Messages
{
    public abstract class PostSendMessageResult
    {
        public abstract T Accept<T>(IPatientSendMessageResultVisitor<T> visitor);
        
        public class Success : PostSendMessageResult
        {
            public PostMessageResponse Response { get; }

            public Success(PostMessageResponse response)
            {
                Response = response;
            }

            public override T Accept<T>(IPatientSendMessageResultVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }

        public class BadGateway : PostSendMessageResult
        {
            public override T Accept<T>(IPatientSendMessageResultVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }
        
        public class Forbidden : PostSendMessageResult
        {
            public override T Accept<T>(IPatientSendMessageResultVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }
        
        public class BadRequest : PostSendMessageResult
        {
            public override T Accept<T>(IPatientSendMessageResultVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }
        
        public class InternalServerError : PostSendMessageResult
        {
            public override T Accept<T>(IPatientSendMessageResultVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }
    }
}