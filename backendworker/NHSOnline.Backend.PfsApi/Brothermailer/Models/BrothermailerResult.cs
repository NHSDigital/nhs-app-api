namespace NHSOnline.Backend.PfsApi.Brothermailer.Models
{
    public abstract class BrothermailerResult
    {
        public abstract T Accept<T>(IBrothermailerResultVisitor<T> visitor);

        public class SuccessfullyRetrieved : BrothermailerResult
        {
            public override T Accept<T>(IBrothermailerResultVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }

        public class Unsuccessful : BrothermailerResult
        {
            public override T Accept<T>(IBrothermailerResultVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }

        public class BrothermailerServiceUnavailable : BrothermailerResult
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