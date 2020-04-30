using NHSOnline.Backend.Support.Configuration;

namespace NHSOnline.Backend.Repository
{
    public interface IRepositoryConfiguration : IValidatable
    {
        string ConnectionString { get; }
        string DatabaseName { get; }
        string CollectionName { get; }
    }
}