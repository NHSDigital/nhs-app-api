using NHSOnline.Backend.MessagesApi.Areas.Messages.Models;
using NHSOnline.Backend.Repository;

namespace NHSOnline.Backend.MessagesApi.Areas.Messages
{
    internal class RepositoryUpdateMessageResultVisitor : IRepositoryUpdateResultVisitor<UserMessage, MessagePatchResult>
    {
        public MessagePatchResult Visit(RepositoryUpdateResult<UserMessage>.NotFound result)
        {
            return new MessagePatchResult.NotFound();
        }

        public MessagePatchResult Visit(RepositoryUpdateResult<UserMessage>.InternalServerError result)
        {
            return new MessagePatchResult.InternalServerError();
        }

        public MessagePatchResult Visit(RepositoryUpdateResult<UserMessage>.RepositoryError result)
        {
            return new MessagePatchResult.InternalServerError();
        }

        public MessagePatchResult Visit(RepositoryUpdateResult<UserMessage>.NoChange result)
        {
            return new MessagePatchResult.Updated();
        }

        public MessagePatchResult Visit(RepositoryUpdateResult<UserMessage>.Updated result)
        {
            return new MessagePatchResult.Updated();
        }
    }
}