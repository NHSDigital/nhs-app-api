using MongoDB.Driver;

namespace NHSOnline.Backend.Repository
{
    public interface IApiMongoClient<TConfiguration> : IMongoClient where TConfiguration : IRepositoryConfiguration
    {
    }
}