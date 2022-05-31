using Microsoft.Azure.Cosmos;

namespace NHSOnline.Backend.Repository.SqlApi
{
    public class CosmosClientWrapper : ICosmosClientWrapper
    {
        private readonly CosmosClient _cosmosClient;

        public CosmosClientWrapper(CosmosClient cosmosClient)
        {
            _cosmosClient = cosmosClient;
        }

        public Container GetContainer(string database, string container)
        {
            return _cosmosClient.GetContainer(database, container);
        }
    }
}