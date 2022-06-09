namespace NHSOnline.Backend.Messages.Areas.Messages.Models
{
    public abstract class SenderResult
    {
        public abstract T Accept<T>(ISenderResultVisitor<T> visitor);

        public class Found : SenderResult
        {
            public Sender Response { get; }

            public Found(Sender response)
            {
                Response = response;
            }

            public override T Accept<T>(ISenderResultVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }

        public class NotFound : SenderResult
        {
            public override T Accept<T>(ISenderResultVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }

        public class BadGateway : SenderResult
        {
            public override T Accept<T>(ISenderResultVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }

        public class InternalServerError : SenderResult
        {
            public override T Accept<T>(ISenderResultVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }
    }
}