using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
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
        private readonly ILogger<OdsCodeLookup> _logger;

        public OdsCodeLookup(IConnectionMultiplexerFactory connectionMultiplexerFactory, ILogger<OdsCodeLookup> logger)
        {
            _connectionMultiplexerFactory =
                connectionMultiplexerFactory ?? throw new ArgumentNullException(nameof(connectionMultiplexerFactory));
            _logger = logger;
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
                _logger.LogError($"Ods code {odsCode} could not be matched to a supported GP System {supplierName}");
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
