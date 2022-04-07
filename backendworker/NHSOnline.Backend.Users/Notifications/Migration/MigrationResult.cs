namespace NHSOnline.Backend.Users.Notifications.Migration
{
    public abstract class MigrationResult
    {
        public abstract T Accept<T>(IMigrationResultVisitor<T> visitor);

        public class Success : MigrationResult
        {
            public string InstallationId { get; }

            public Success(string installationId)
            {
                InstallationId = installationId;
            }

            public override T Accept<T>(IMigrationResultVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }

        public class BadRequest : MigrationResult
        {
            public string ErrorMessage { get; }

            public BadRequest(string errorMessage)
            {
                ErrorMessage = errorMessage;
            }

            public override T Accept<T>(IMigrationResultVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }

        public class BadGateway : MigrationResult
        {
            public override T Accept<T>(IMigrationResultVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }

        public class InternalServerError : MigrationResult
        {
            public override T Accept<T>(IMigrationResultVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }
    }
}
