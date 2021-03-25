using System;
using NHSOnline.Backend.MessagesApi.Areas.Messages.Models;
using NHSOnline.Backend.Repository;

namespace NHSOnline.Backend.MessagesApi.Areas.Messages
{
    public class RepositoryFindMessageLinkClickedResultVisitor : IRepositoryFindResultVisitor<UserMessage, MessageLinkClickedResult>
    {
        public MessageLinkClickedResult Visit(RepositoryFindResult<UserMessage>.NotFound result)
        {
            return new MessageLinkClickedResult.BadRequest();
        }

        public MessageLinkClickedResult Visit(RepositoryFindResult<UserMessage>.Found result)
        {
            throw new NotImplementedException();
        }

        public MessageLinkClickedResult Visit(RepositoryFindResult<UserMessage>.RepositoryError result)
        {
            return new MessageLinkClickedResult.BadGateway();
        }
    }
}
