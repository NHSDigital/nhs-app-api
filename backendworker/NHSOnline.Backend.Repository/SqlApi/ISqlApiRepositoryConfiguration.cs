using NHSOnline.Backend.Support.Configuration;

namespace NHSOnline.Backend.Repository.SqlApi
{
    public interface ISqlApiRepositoryConfiguration : IValidatable
    {
        public string DatabaseName { get; }
        public string ContainerName { get; }
    }
}