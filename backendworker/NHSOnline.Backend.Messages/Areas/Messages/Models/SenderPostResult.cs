using NHSOnline.Backend.Messages.Repository;

namespace NHSOnline.Backend.Messages.Areas.Messages.Models
{
    public abstract class SenderPostResult
    {
        public abstract T Accept<T>(ISenderPostResultVisitor<T> visitor);

        public class BadGateway : SenderPostResult
        {
            public override T Accept<T>(ISenderPostResultVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }

        public class InternalServerError : SenderPostResult
        {
            public override T Accept<T>(ISenderPostResultVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }

        public class Created : SenderPostResult
        {
            public DbSender DbSender { get; }

            public Created(DbSender sender)
            {
                DbSender = sender;
            }

            public override T Accept<T>(ISenderPostResultVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }
    }
}