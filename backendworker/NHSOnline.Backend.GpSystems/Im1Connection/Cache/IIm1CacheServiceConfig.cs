namespace NHSOnline.Backend.GpSystems.Im1Connection.Cache
{
    public interface IIm1CacheServiceConfig
    {
        string DatabaseName { get; }
        string CollectionName { get; }
        string SecondaryDatabaseName { get; }
        string SecondaryCollectionName { get; }
    }
}