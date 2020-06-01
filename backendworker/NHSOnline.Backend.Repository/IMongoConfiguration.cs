namespace NHSOnline.Backend.Repository
{
    public interface IMongoConfiguration
    {
        string ConnectionString { get; }
        string DatabaseName { get; }
        string CollectionName { get; }
    }
}