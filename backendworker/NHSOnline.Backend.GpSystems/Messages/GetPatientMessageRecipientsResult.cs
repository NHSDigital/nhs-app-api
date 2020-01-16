using NHSOnline.Backend.GpSystems.Suppliers.Emis.Models.Messages;

namespace NHSOnline.Backend.GpSystems.Messages
{
    public abstract class GetPatientMessageRecipientsResult
    {
        public abstract T Accept<T>(IPatientMessageRecipientsResultVisitor<T> visitor);
        
        public class Success : GetPatientMessageRecipientsResult
        {
            public MessageRecipientsGetResponse Response { get; }

            public Success(MessageRecipientsGetResponse response)
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