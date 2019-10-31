namespace NHSOnline.Backend.MessagesApi.Areas.Messages.Models
{
    public abstract class MessagesResult
    {
        public abstract T Accept<T>(IMessagesResultVisitor<T> visitor);

        public class Some : MessagesResult
        {
            public MessagesResponse Response { get; }

            public Some(MessagesResponse response)
            {
                Response = response;
            }

            public override T Accept<T>(IMessagesResultVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }

        public class None : MessagesResult
        {
            public override T Accept<T>(IMessagesResultVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }

        public class BadGateway : MessagesResult
        {
            public override T Accept<T>(IMessagesResultVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }
        
        public class BadRequest : MessagesResult
        {
            public override T Accept<T>(IMessagesResultVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }
        
        public class InternalServerError : MessagesResult
        {
            public override T Accept<T>(IMessagesResultVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }
    }
}