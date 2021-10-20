namespace NHSOnline.Backend.PfsApi.Messages
{
    public abstract class MessagesResult
    {
        public class NoActionTaken : MessagesResult
        {
        }

        public class Success : MessagesResult
        {
            public string MessageId { get; }

            public Success(string messageId)
            {
                MessageId = messageId;
            }
        }


        public class InternalServerError : MessagesResult
        {
        }
    }
}
