using System;
using NHSOnline.Backend.MessagesApi.Areas.Messages.Models;
using NHSOnline.Backend.Repository;

namespace NHSOnline.Backend.MessagesApi.Areas.Messages
{
    internal class RepositoryFindMessagePatchResultVisitor : IRepositoryFindResultVisitor<UserMessage, MessagePatchResult>
    {
        public MessagePatchResult Visit(RepositoryFindResult<UserMessage>.NotFound result)
        {
            return new MessagePatchResult.NotFound();
        }

        public MessagePatchResult Visit(RepositoryFindResult<UserMessage>.RepositoryError result)
        {
            return new MessagePatchResult.BadGateway();
        }

        public MessagePatchResult Visit(RepositoryFindResult<UserMessage>.Found result)
        {
            throw new NotImplementedException();
        }
    }
}