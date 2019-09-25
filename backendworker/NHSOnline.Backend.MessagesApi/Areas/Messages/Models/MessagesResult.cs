using System.Collections.Generic;

namespace NHSOnline.Backend.MessagesApi.Areas.Messages.Models
{
    public abstract class MessagesResult
    {
        public abstract T Accept<T>(IMessagesResultVisitor<T> visitor);

        public class Some : MessagesResult
        {
            public IEnumerable<UserMessage> Messages { get; }

            public Some(IEnumerable<UserMessage> messages)
            {
                Messages = messages;
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
        
        public class InternalServerError : MessagesResult
        {
            public override T Accept<T>(IMessagesResultVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }
    }
}