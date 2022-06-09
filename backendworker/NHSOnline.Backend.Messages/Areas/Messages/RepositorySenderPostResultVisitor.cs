using NHSOnline.Backend.Messages.Areas.Messages.Models;
using NHSOnline.Backend.Messages.Repository;
using NHSOnline.Backend.Repository;

namespace NHSOnline.Backend.Messages.Areas.Messages
{
    internal class RepositorySenderPostResultVisitor : IRepositoryCreateResultVisitor<DbSender, SenderPostResult>
    {
        public SenderPostResult Visit(RepositoryCreateResult<DbSender>.Created result)
        {
            return new SenderPostResult.Success(result.Record);
        }
        
        public SenderPostResult Visit(RepositoryCreateResult<DbSender>.RepositoryError result)
        {
            return new SenderPostResult.BadGateway();
        }
    }
}