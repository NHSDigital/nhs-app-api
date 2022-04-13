namespace NHSOnline.Backend.Messages.Areas.Messages.Models
{
    public abstract class MessageLinkClickedResult
    {
        public abstract T Accept<T>(IMessageLinkClickedResultVisitor<T> visitor);

        public class BadGateway : MessageLinkClickedResult
        {
            public override T Accept<T>(IMessageLinkClickedResultVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }

        public class BadRequest : MessageLinkClickedResult
        {
            public override T Accept<T>(IMessageLinkClickedResultVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }

        public class InternalServerError : MessageLinkClickedResult
        {
            public override T Accept<T>(IMessageLinkClickedResultVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }

        public class Success : MessageLinkClickedResult
        {
            public override T Accept<T>(IMessageLinkClickedResultVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }
    }
}
