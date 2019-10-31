namespace NHSOnline.Backend.MessagesApi.Areas.Messages.Models
{
    public abstract class MessagePatchResult
    {
        public abstract T Accept<T>(IMessagePatchResultVisitor<T> visitor);

        public class BadRequest : MessagePatchResult
        {
            public override T Accept<T>(IMessagePatchResultVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }
        
        public class BadGateway : MessagePatchResult
        {
            public override T Accept<T>(IMessagePatchResultVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }
        
        public class InternalServerError : MessagePatchResult
        {
            public override T Accept<T>(IMessagePatchResultVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }

        public class NotFound : MessagePatchResult
        {
            public override T Accept<T>(IMessagePatchResultVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }

        public class Updated : MessagePatchResult
        {
            public override T Accept<T>(IMessagePatchResultVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }
    }
}