namespace NHSOnline.Backend.Messages.Areas.Messages.Models
{
    public abstract class MessagesMetadataResult
    {
        public abstract T Accept<T>(IMessagesMetadataResultVisitor<T> visitor);

        public class Found : MessagesMetadataResult
        {
            public MessagesMetadataResponse Response { get; }

            public Found(MessagesMetadataResponse response)
            {
                Response = response;
            }

            public override T Accept<T>(IMessagesMetadataResultVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }

        public class BadGateway : MessagesMetadataResult
        {
            public override T Accept<T>(IMessagesMetadataResultVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }

        public class InternalServerError : MessagesMetadataResult
        {
            public override T Accept<T>(IMessagesMetadataResultVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }
    }
}