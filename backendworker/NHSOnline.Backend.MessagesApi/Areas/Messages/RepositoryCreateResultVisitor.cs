using NHSOnline.Backend.MessagesApi.Areas.Messages.Models;
using NHSOnline.Backend.Repository;

namespace NHSOnline.Backend.MessagesApi.Areas.Messages
{
    internal class RepositoryCreateResultVisitor : IRepositoryCreateResultVisitor<UserMessage, MessageResult>
    {

        public MessageResult Visit(RepositoryCreateResult<UserMessage>.InternalServerError result)
        {
            return new MessageResult.InternalServerError();
        }

        public MessageResult Visit(RepositoryCreateResult<UserMessage>.RepositoryError result)
        {
            return new MessageResult.InternalServerError();
        }

        public MessageResult Visit(RepositoryCreateResult<UserMessage>.Created result)
        {
            return new MessageResult.Success();
        }
    }
}