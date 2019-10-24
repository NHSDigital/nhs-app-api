using MongoDB.Driver;

namespace NHSOnline.Backend.Support.Repository
{
    public interface IApiMongoClient<TConfiguration> : IMongoClient where TConfiguration : IMongoConfiguration
    {
    }
}