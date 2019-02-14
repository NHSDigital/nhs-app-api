using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Support;
using NHSOnline.Backend.Support.Logging;
using StackExchange.Redis;

namespace NHSOnline.Backend.Worker
{
    public interface IOdsCodeLookup
    {
        Task<Option<Supplier>> LookupSupplier(string odsCode);
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

        public async Task<Option<Supplier>> LookupSupplier(string odsCode)
        {
            _logger.LogInformation("Looking up ODS Code {odsCode}", odsCode);

            if (string.IsNullOrWhiteSpace(odsCode))
            {
                return Option.None<Supplier>();
            }

            var supplierName = await GetSupplierNameFromRedis(odsCode);

            if (!Enum.TryParse(supplierName, true, out Supplier supplierEnum))
            {
                _logger.LogError($"Ods code {odsCode} could not be matched to a supported GP System {supplierName}");
                return Option.None<Supplier>();
            }

            return Option.Some(supplierEnum);
        }

        private async Task<RedisValue> GetSupplierNameFromRedis(string odsCode)
        {
            try
            {
                _logger.LogEnter();
                var multiplexer = _connectionMultiplexerFactory.GetMultiplexer(ConnectionMultiplexerName.OdsCodeLookup);
                var database = multiplexer.GetDatabase();

                using (_logger.WithTimer("Retrieving supplier name from Redis"))
                {
                    return await database.StringGetAsync(odsCode);
                }
            }
            finally
            {
                _logger.LogExit();
            }
        }
    }
}
