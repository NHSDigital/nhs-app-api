using System.Linq;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Linq;

namespace NHSOnline.Backend.Repository.SqlApi
{
    public class CosmosLinqQuery : ICosmosLinqQuery
    {
        public FeedIterator<T> GetFeedIterator<T>(IQueryable<T> query)
        {
            return query.ToFeedIterator();
        }
    }
}