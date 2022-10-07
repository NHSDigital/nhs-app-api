namespace NHSOnline.Backend.Messages.Areas.Messages.Models
{
    public abstract class UnreadMessageCountResult
    {
        public abstract T Accept<T>(IUnreadMessageCountResultVisitor<T> visitor);

        public class Success : UnreadMessageCountResult
        {
            public long UnreadMessageCount { get; }

            public Success(long unreadMessageCount)
            {
                UnreadMessageCount = unreadMessageCount;
            }

            public override T Accept<T>(IUnreadMessageCountResultVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }

        public class Failure : UnreadMessageCountResult
        {
            public override T Accept<T>(IUnreadMessageCountResultVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }
    }
}