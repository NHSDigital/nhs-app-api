using Microsoft.Azure.Cosmos;

namespace NHSOnline.Backend.Repository.SqlApi
{
    public interface ICosmosClientWrapper
    {
        public Container GetContainer(string database, string container);
    }
}