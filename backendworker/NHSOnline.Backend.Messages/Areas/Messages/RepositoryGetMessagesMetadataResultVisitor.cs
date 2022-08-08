using NHSOnline.Backend.Messages.Areas.Messages.Models;
using NHSOnline.Backend.Repository;

namespace NHSOnline.Backend.Messages.Areas.Messages
{
    public class RepositoryGetMessagesMetadataResultVisitor : IRepositoryCountResultVisitor<MessagesMetadataResult>
    {
        public MessagesMetadataResult Visit(RepositoryCountResult.Found result)
        {
            return new MessagesMetadataResult.Found(new MessagesMetadataResponse
            {
                MessagesMetadata = new MessagesMetadata
                {
                    UnreadMessageCount = result.Count
                }
            });
        }

        public MessagesMetadataResult Visit(RepositoryCountResult.RepositoryError result)
        {
            return new MessagesMetadataResult.BadGateway();
        }
    }
}