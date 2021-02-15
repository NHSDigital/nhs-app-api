namespace NHSOnline.Backend.GpSystems.LinkedAccounts
{
    public abstract class SwitchAccountResult
    {
        public abstract T Accept<T>(ISwitchAccountResultVisitor<T> visitor);

        public class Success : SwitchAccountResult
        {
            public string ToNhsNumber { get; set; }

            public override T Accept<T>(ISwitchAccountResultVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }

        public class AlreadyAuthenticated : SwitchAccountResult
        {
            public string AuthenticatedId { get; }

            public AlreadyAuthenticated(string authenticatedId)
            {
                AuthenticatedId = authenticatedId;
            }

            public override T Accept<T>(ISwitchAccountResultVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }

        public class NotFound : SwitchAccountResult
        {
            public string AttemptedIdToSwitchTo { get; }

            public NotFound(string attemptedIdToSwitchTo)
            {
                AttemptedIdToSwitchTo = attemptedIdToSwitchTo;
            }

            public override T Accept<T>(ISwitchAccountResultVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }

        public class Failure : SwitchAccountResult
        {
            public string AttemptedIdToSwitchTo { get; }

            public Failure(string attemptedIdToSwitchTo)
            {
                AttemptedIdToSwitchTo = attemptedIdToSwitchTo;
            }

            public override T Accept<T>(ISwitchAccountResultVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }
    }
}