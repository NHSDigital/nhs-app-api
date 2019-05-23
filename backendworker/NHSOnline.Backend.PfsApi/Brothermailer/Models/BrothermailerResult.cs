namespace NHSOnline.Backend.PfsApi.Brothermailer.Models
{
    public abstract class BrothermailerResult
    {
        public abstract T Accept<T>(IBrothermailerResultVisitor<T> visitor);

        public class Success : BrothermailerResult
        {
            public override T Accept<T>(IBrothermailerResultVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }

        public class InternalServerError : BrothermailerResult
        {
            public override T Accept<T>(IBrothermailerResultVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }

        public class BadGateway : BrothermailerResult
        {
            public override T Accept<T>(IBrothermailerResultVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }
        
        public class BadRequest : BrothermailerResult
        {
            public override T Accept<T>(IBrothermailerResultVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }
    }
}