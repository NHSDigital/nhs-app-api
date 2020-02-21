namespace NHSOnline.Backend.GpSystems.Messages
{
    public abstract class DeletePatientMessageResult
    {
        public abstract T Accept<T>(IPatientMessageDeleteResultVisitor<T> visitor);

        public class Success : DeletePatientMessageResult
        {
            public override T Accept<T>(IPatientMessageDeleteResultVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }

        public class BadGateway : DeletePatientMessageResult
        {
            public override T Accept<T>(IPatientMessageDeleteResultVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }

        public class Forbidden : DeletePatientMessageResult
        {
            public override T Accept<T>(IPatientMessageDeleteResultVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }

        public class BadRequest : DeletePatientMessageResult
        {
            public override T Accept<T>(IPatientMessageDeleteResultVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }

        public class InternalServerError : DeletePatientMessageResult
        {
            public override T Accept<T>(IPatientMessageDeleteResultVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }
    }
}