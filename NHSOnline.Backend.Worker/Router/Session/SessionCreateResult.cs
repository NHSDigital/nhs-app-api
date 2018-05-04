using System;
using NHSOnline.Backend.Worker.Session;

namespace NHSOnline.Backend.Worker.Router.Session
{
    public abstract class SessionCreateResult
    {
        private SessionCreateResult()
        {
        }

        public abstract T Accept<T>(ISessionCreateResultVisitor<T> visitor);

        public class SuccessfullyCreated : SessionCreateResult
        {
            public UserSession UserSession { get; }
            public string GivenName { get; }
            public string FamilyName { get; }
           
            

            public SuccessfullyCreated(
                string givenName, 
                string familyName, 
                UserSession userSession)
            {
                GivenName = givenName;
                FamilyName = familyName;
                UserSession = userSession;
            }

            public override T Accept<T>(ISessionCreateResultVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }

        public class InvalidIm1ConnectionToken : SessionCreateResult
        {
            public override T Accept<T>(ISessionCreateResultVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }

        public class SupplierSystemUnavailable : SessionCreateResult
        {
            public override T Accept<T>(ISessionCreateResultVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }
    }
}