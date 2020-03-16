using NHSOnline.Backend.GpSystems.Messages.Models;

namespace NHSOnline.Backend.GpSystems.Messages
{
    public abstract class GetPatientMessageRecipientsResult
    {
        public abstract T Accept<T>(IPatientMessageRecipientsResultVisitor<T> visitor);

        public class Success : GetPatientMessageRecipientsResult
        {
            public MessageRecipientsResponse Response { get; }

            public Success(MessageRecipientsResponse response)
            {
                Response = response;
            }

            public override T Accept<T>(IPatientMessageRecipientsResultVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }

        public class BadGateway : GetPatientMessageRecipientsResult
        {
            public override T Accept<T>(IPatientMessageRecipientsResultVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }

        public class Forbidden : GetPatientMessageRecipientsResult
        {
            public override T Accept<T>(IPatientMessageRecipientsResultVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }

        public class BadRequest : GetPatientMessageRecipientsResult
        {
            public override T Accept<T>(IPatientMessageRecipientsResultVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }

        public class InternalServerError : GetPatientMessageRecipientsResult
        {
            public override T Accept<T>(IPatientMessageRecipientsResultVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }
    }
}