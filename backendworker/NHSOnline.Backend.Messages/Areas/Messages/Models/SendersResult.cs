namespace NHSOnline.Backend.Messages.Areas.Messages.Models
{
    public abstract class SendersResult
    {
        public abstract T Accept<T>(ISendersResultVisitor<T> visitor);

        public class Found : SendersResult
        {
            public SendersResponse Response { get; }

            public Found(SendersResponse response)
            {
                Response = response;
            }

            public override T Accept<T>(ISendersResultVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }

        public class None : SendersResult
        {
            public override T Accept<T>(ISendersResultVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }

        public class BadGateway : SendersResult
        {
            public override T Accept<T>(ISendersResultVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }

        public class InternalServerError : SendersResult
        {
            public override T Accept<T>(ISendersResultVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }
    }
}