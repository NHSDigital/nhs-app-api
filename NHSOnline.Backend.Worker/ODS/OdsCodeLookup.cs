using System;
using System.Threading.Tasks;
using NHSOnline.Backend.Worker.Suppliers;
using StackExchange.Redis;

namespace NHSOnline.Backend.Worker.Ods
{
    public class OdsCodeLookup : IOdsCodeLookup
    {
        private readonly IConnectionMultiplexerFactory _connectionMultiplexerFactory;

        public OdsCodeLookup(IConnectionMultiplexerFactory connectionMultiplexerFactory)
        {
            _connectionMultiplexerFactory =
                connectionMultiplexerFactory ?? throw new ArgumentNullException(nameof(connectionMultiplexerFactory));
        }

        public async Task<SupplierEnum> LookupSupplier(string odsCode)
        {
            if (string.IsNullOrWhiteSpace(odsCode))
            {
                throw new ArgumentNullException(nameof(odsCode));
            }

            var supplierName = await GetSupplierNameFromRedis(odsCode);

            if (!Enum.TryParse(supplierName, true, out SupplierEnum supplierEnum))
            {
                var errmsg = string.Format(ExceptionMessages.OdsCodeLookupUnknownSupplierCode, odsCode, supplierName);
                throw new OdsCodeLookupException(errmsg, odsCode);
            }

            return supplierEnum;
        }

        private async Task<RedisValue> GetSupplierNameFromRedis(string odsCode)
        {
            var multiplexer = _connectionMultiplexerFactory.GetMultiplexer(ConnectionMultiplexerName.OdsCodeLookup);
            var database = multiplexer.GetDatabase();
            var redisValue = await database.StringGetAsync(odsCode);

            if (redisValue == default(RedisValue))
            {
                var errmsg = string.Format(ExceptionMessages.OdsCodeLookupUnknownOdsCode, odsCode);
                throw new OdsCodeLookupException(errmsg, odsCode);
            }

            return redisValue;
        }
    }
}