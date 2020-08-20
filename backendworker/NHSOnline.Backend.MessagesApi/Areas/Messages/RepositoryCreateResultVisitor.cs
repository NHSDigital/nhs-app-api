using NHSOnline.Backend.MessagesApi.Areas.Messages.Models;
using NHSOnline.Backend.Repository;

namespace NHSOnline.Backend.MessagesApi.Areas.Messages
{
    internal class RepositoryCreateResultVisitor : IRepositoryCreateResultVisitor<UserMessage, MessageResult>
    {
        public MessageResult Visit(RepositoryCreateResult<UserMessage>.RepositoryError result)
        {
            return new MessageResult.BadGateway();
        }

        public MessageResult Visit(RepositoryCreateResult<UserMessage>.Created result)
        {
            return new MessageResult.Success(result.Record.Id.ToString());
        }
    }
}