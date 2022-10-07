using NHSOnline.Backend.Messages.Areas.Messages.Models;
using NHSOnline.Backend.Repository;

namespace NHSOnline.Backend.Messages.Areas.Messages
{
    public class RepositoryUnreadMessageCountResultVisitor : IRepositoryCountResultVisitor<UnreadMessageCountResult>
    {
        public UnreadMessageCountResult Visit(RepositoryCountResult.Found result)
            => new UnreadMessageCountResult.Success(result.Count);

        public UnreadMessageCountResult Visit(RepositoryCountResult.RepositoryError result)
            => new UnreadMessageCountResult.Failure();
    }
}