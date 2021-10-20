using NHSOnline.Backend.MessagesApi.Areas.Messages.Models;
using NHSOnline.Backend.Repository;

namespace NHSOnline.Backend.MessagesApi.Areas.Messages
{
    internal class RepositoryCreateResultVisitor : IRepositoryCreateResultVisitor<UserMessage, AddMessageResult>
    {
        public AddMessageResult Visit(RepositoryCreateResult<UserMessage>.RepositoryError result)
        {
            return new AddMessageResult.BadGateway();
        }

        public AddMessageResult Visit(RepositoryCreateResult<UserMessage>.Created result)
        {
            return new AddMessageResult.Success(result.Record);
        }
    }
}