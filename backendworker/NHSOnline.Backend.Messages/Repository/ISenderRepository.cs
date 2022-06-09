using System.Threading.Tasks;
using NHSOnline.Backend.Repository;

namespace NHSOnline.Backend.Messages.Repository
{
    public interface ISenderRepository
    {
        Task<RepositoryCreateResult<DbSender>> CreateOrUpdate(DbSender sender);
        Task<RepositoryFindResult<DbSender>> Find(string senderId);
    }
}