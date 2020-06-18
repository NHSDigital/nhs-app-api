namespace NHSOnline.Backend.UsersApi.Notifications
{
    public abstract class RegistrationResult
    {
        public abstract T Accept<T>(IRegistrationResultVisitor<T> visitor);

        public class Success : RegistrationResult
        {
            public Success(NotificationRegistrationResult response)
            {
                Response = response;
            }

            public NotificationRegistrationResult Response { get; }

            public override T Accept<T>(IRegistrationResultVisitor<T> visitor)
            {
               return visitor.Visit(this);
            }
        }

        public class BadGateway : RegistrationResult
        {
            public override T Accept<T>(IRegistrationResultVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }

        public class InternalServerError : RegistrationResult
        {
            public override T Accept<T>(IRegistrationResultVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }
    }
}