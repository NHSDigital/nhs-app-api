namespace NHSOnline.Backend.UsersApi.Notifications
{
    public abstract class DeleteRegistrationResult
    {
        public abstract T Accept<T>(IDeleteRegistrationResultVisitor<T> visitor);

        public class Success : DeleteRegistrationResult
        {
            public override T Accept<T>(IDeleteRegistrationResultVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }

        public class NotFound : DeleteRegistrationResult
        {
            public override T Accept<T>(IDeleteRegistrationResultVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }

        public class BadGateway : DeleteRegistrationResult
        {
            public override T Accept<T>(IDeleteRegistrationResultVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }

        public class InternalServerError : DeleteRegistrationResult
        {
            public override T Accept<T>(IDeleteRegistrationResultVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }
    }
}