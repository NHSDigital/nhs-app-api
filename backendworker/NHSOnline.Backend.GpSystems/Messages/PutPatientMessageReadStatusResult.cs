namespace NHSOnline.Backend.GpSystems.Messages
{
    public abstract class PutPatientMessageReadStatusResult
    {
        public abstract T Accept<T>(IPatientMessageUpdateReadStatusResultVisitor<T> visitor);

        public class Success : PutPatientMessageReadStatusResult
        {
            public override T Accept<T>(IPatientMessageUpdateReadStatusResultVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }

        public class BadGateway : PutPatientMessageReadStatusResult
        {
            public override T Accept<T>(IPatientMessageUpdateReadStatusResultVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }

        public class Forbidden : PutPatientMessageReadStatusResult
        {
            public override T Accept<T>(IPatientMessageUpdateReadStatusResultVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }

        public class BadRequest : PutPatientMessageReadStatusResult
        {
            public override T Accept<T>(IPatientMessageUpdateReadStatusResultVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }

        public class InternalServerError : PutPatientMessageReadStatusResult
        {
            public override T Accept<T>(IPatientMessageUpdateReadStatusResultVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }
    }
}