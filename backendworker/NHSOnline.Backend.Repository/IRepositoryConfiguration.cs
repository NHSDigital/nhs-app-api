using NHSOnline.Backend.Support;
using NHSOnline.Backend.Support.Settings;

namespace NHSOnline.Backend.Repository
{
    public interface IRepositoryConfiguration : IValidatable
    {
        string ConnectionString { get; }
        string DatabaseName { get; }
        string CollectionName { get; }
    }
}