namespace NHSOnline.Backend.MessagesApi.Areas.Messages.Models
{
    public abstract class MessageResult
    {
        public abstract T Accept<T>(IMessageResultVisitor<T> visitor);

        public class Success : MessageResult
        {
            public override T Accept<T>(IMessageResultVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }
        
        public class BadGateway : MessageResult
        {
            public override T Accept<T>(IMessageResultVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }
               
        public class BadRequest : MessageResult
        {
            public override T Accept<T>(IMessageResultVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }
        
        public class InternalServerError : MessageResult
        {
            public override T Accept<T>(IMessageResultVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }
    }
}