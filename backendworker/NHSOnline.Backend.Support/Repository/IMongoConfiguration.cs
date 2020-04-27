namespace NHSOnline.Backend.Support.Repository
{
    public interface IMongoConfiguration
    {
        string ConnectionString { get; }
        string DatabaseName { get; }
        string CollectionName { get; }
    }
}