using System;

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
            public Guid AuthenticatedId { get; }

            public AlreadyAuthenticated(Guid authenticatedId)
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
            public Guid AttemptedIdToSwitchTo { get; }

            public NotFound(Guid attemptedIdToSwitchTo)
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
            public Guid AttemptedIdToSwitchTo { get; }

            public Failure(Guid attemptedIdToSwitchTo)
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