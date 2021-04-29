namespace NHSOnline.Backend.MessagesApi.Areas.Messages.Models
{
    public abstract class MessagePatchResult
    {
        public abstract T Accept<T>(IMessagePatchResultVisitor<T> visitor);

        public class BadRequest : MessagePatchResult
        {
            public override T Accept<T>(IMessagePatchResultVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }

        public class BadGateway : MessagePatchResult
        {
            public override T Accept<T>(IMessagePatchResultVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }

        public class InternalServerError : MessagePatchResult
        {
            public override T Accept<T>(IMessagePatchResultVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }

        public class NotFound : MessagePatchResult
        {
            public override T Accept<T>(IMessagePatchResultVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }

        public class NoChange : MessagePatchResult
        {
            public override T Accept<T>(IMessagePatchResultVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }

        public class Updated : MessagePatchResult
        {
            public string Id { get; }
            public string CommunicationId { get; }
            public string TransmissionId { get; }
            public string CampaignId { get; }
            public string SupplierId { get; }

            public Updated(
                string id,
                string communicationId,
                string transmissionId,
                string campaignId,
                string supplierId
            )
            {
                Id = id;
                CommunicationId = communicationId;
                TransmissionId = transmissionId;
                CampaignId = campaignId;
                SupplierId = supplierId;
            }

            public override T Accept<T>(IMessagePatchResultVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }
    }
}