namespace NHSOnline.Backend.MessagesApi.Areas.Messages.Models
{
    public abstract class AddMessageResult
    {
        public abstract T Accept<T>(IAddMessageResultVisitor<T> visitor);

        public class Success : AddMessageResult
        {
            public UserMessage UserMessage { get; }

            public Success(UserMessage userMessage)
            {
                UserMessage = userMessage;
            }

            public override T Accept<T>(IAddMessageResultVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }

        public class BadGateway : AddMessageResult
        {
            public override T Accept<T>(IAddMessageResultVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }

        public class BadRequest : AddMessageResult
        {
            public override T Accept<T>(IAddMessageResultVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }

        public class InternalServerError : AddMessageResult
        {
            public override T Accept<T>(IAddMessageResultVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }
    }
}