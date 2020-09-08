using System.Collections.Generic;

namespace NHSOnline.Backend.UsersApi.Notifications
{
    public abstract class FindRegistrationsResult
    {
        public abstract T Accept<T>(IFindRegistrationsResultVisitor<T> visitor);

        public class Found : FindRegistrationsResult
        {
            public Found(ICollection<string> registrationIds)
            {
                RegistrationIds = registrationIds;
            }

            public ICollection<string> RegistrationIds { get; }

            public override T Accept<T>(IFindRegistrationsResultVisitor<T> visitor)
            {
               return visitor.Visit(this);
            }
        }

        public class NotFound : FindRegistrationsResult
        {
            public override T Accept<T>(IFindRegistrationsResultVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }

        public class BadGateway : FindRegistrationsResult
        {
            public override T Accept<T>(IFindRegistrationsResultVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }

        public class InternalServerError : FindRegistrationsResult
        {
            public override T Accept<T>(IFindRegistrationsResultVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }
    }
}