using System;
using System.Threading.Tasks;
using NHSOnline.Backend.Worker.Support;
using StackExchange.Redis;

namespace NHSOnline.Backend.Worker
{
    public interface IOdsCodeLookup
    {
        Task<Option<SupplierEnum>> LookupSupplier(string odsCode);
    }

    public class OdsCodeLookup : IOdsCodeLookup
    {
        private readonly IConnectionMultiplexerFactory _connectionMultiplexerFactory;

        public OdsCodeLookup(IConnectionMultiplexerFactory connectionMultiplexerFactory)
        {
            _connectionMultiplexerFactory =
                connectionMultiplexerFactory ?? throw new ArgumentNullException(nameof(connectionMultiplexerFactory));
        }

        public async Task<Option<SupplierEnum>> LookupSupplier(string odsCode)
        {
            if (string.IsNullOrWhiteSpace(odsCode))
            {
                return Option.None<SupplierEnum>();
            }

            var supplierName = await GetSupplierNameFromRedis(odsCode);

            if (!Enum.TryParse(supplierName, true, out SupplierEnum supplierEnum))
            {
                return Option.None<SupplierEnum>();
            }

            return Option.Some(supplierEnum);
        }

        private async Task<RedisValue> GetSupplierNameFromRedis(string odsCode)
        {
            var multiplexer = _connectionMultiplexerFactory.GetMultiplexer(ConnectionMultiplexerName.OdsCodeLookup);
            var database = multiplexer.GetDatabase();
            var redisValue = await database.StringGetAsync(odsCode);
            return redisValue;
        }
    }
}
