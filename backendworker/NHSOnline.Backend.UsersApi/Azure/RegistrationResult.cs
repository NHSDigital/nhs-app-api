namespace NHSOnline.Backend.UsersApi.Azure
{
    public abstract class RegistrationResult
    {
        public abstract T Accept<T>(IRegistrationResultVisitor<T> visitor);

        public class Success : RegistrationResult
        {
            public AzureRegistrationResponse Response { get; }

            public Success(AzureRegistrationResponse response)
            {
                Response = response;
            }

            public override T Accept<T>(IRegistrationResultVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }

        public class Failure : RegistrationResult
        {
            public override T Accept<T>(IRegistrationResultVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }
    }
}
