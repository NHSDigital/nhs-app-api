using NHSOnline.Backend.Messages.Areas.Messages.Models;
using NHSOnline.Backend.Repository;

namespace NHSOnline.Backend.Messages.Areas.Messages
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