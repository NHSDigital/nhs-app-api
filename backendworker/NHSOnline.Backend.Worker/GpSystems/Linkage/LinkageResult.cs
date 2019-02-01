using NHSOnline.Backend.Worker.GpSystems.Linkage.Models;

namespace NHSOnline.Backend.Worker.GpSystems.Linkage
{
    public abstract class LinkageResult
    {
        public abstract T Accept<T>(ILinkageResultVisitor<T> visitor);

        public class SuccessfullyRetrieved : LinkageResult
        {
            public LinkageResponse Response { get; }

            public SuccessfullyRetrieved(LinkageResponse response)
            {
                Response = response;
            }

            public override T Accept<T>(ILinkageResultVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }

        public class SuccessfullyCreated : LinkageResult
        {
            public LinkageResponse Response { get; }

            public SuccessfullyCreated(LinkageResponse response)
            {
                Response = response;
            }

            public override T Accept<T>(ILinkageResultVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }

        public class InternalServerError : LinkageResult
        {
            public override T Accept<T>(ILinkageResultVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }

        public class SupplierSystemUnavailable : LinkageResult
        {
            public override T Accept<T>(ILinkageResultVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }

        public class ErrorCreatingPatientWhoAlreadyHasAnOnlineAccount : LinkageResult
        {
            public override T Accept<T>(ILinkageResultVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }

        public class NotFoundErrorRetrievingNhsUser : LinkageResult
        {
            public override T Accept<T>(ILinkageResultVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }

        public class PracticeNotLive : LinkageResult
        {
            public override T Accept<T>(ILinkageResultVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }

        public class PatientMarkedAsArchived : LinkageResult
        {
            public override T Accept<T>(ILinkageResultVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }

        public class PatientNonCompetentOrUnderMinimumAge : LinkageResult
        {
            public override T Accept<T>(ILinkageResultVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }

        public class AccountStatusInvalid : LinkageResult
        {
            public override T Accept<T>(ILinkageResultVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }

        public class PatientNotRegisteredAtPractice : LinkageResult
        {
            public override T Accept<T>(ILinkageResultVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }

        public class NoRegisteredOnlineUserFound : LinkageResult
        {
            public override T Accept<T>(ILinkageResultVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }

        public class NotFoundErrorCreatingNhsUser : LinkageResult
        {
            public override T Accept<T>(ILinkageResultVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }

        public class BadRequestErrorRetrievingNhsUser : LinkageResult
        {
            public override T Accept<T>(ILinkageResultVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }

        public class BadRequestErrorCreatingNhsUser : LinkageResult
        {
            public override T Accept<T>(ILinkageResultVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }

        public class LinkageKeyRevoked : LinkageResult
        {
            public override T Accept<T>(ILinkageResultVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }

        public class ForbiddenErrorRetrievingNhsUser : LinkageResult
        {
            public override T Accept<T>(ILinkageResultVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }

        public class SuccessfullyRetrievedAlreadyExists : LinkageResult
        {
            public LinkageResponse Response { get; }

            public SuccessfullyRetrievedAlreadyExists(LinkageResponse response)
            {
                Response = response;
            }

            public override T Accept<T>(ILinkageResultVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }
    }
}
