using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;

namespace NHSOnline.Backend.Support
{
    public abstract class OneToOneEnumMapper<TEnum> : EnumMapper<TEnum>
        where TEnum : struct
    {

        private bool _isInitialised;
        private Dictionary<TEnum, string> _fromMappingTable;
        
        protected OneToOneEnumMapper(ILogger<TEnum> logger) : base(logger)
        {
        }
        
        protected abstract Dictionary<string, TEnum>  MappingTable { get; }

        protected sealed override Dictionary<string, TEnum> ToMappingTable => MappingTable;
        protected sealed override Dictionary<TEnum, string> FromMappingTable
        {
            get
            {
                if (_isInitialised) return _fromMappingTable;
                
                _fromMappingTable = ToMappingTable.ToDictionary(c => c.Value, c => c.Key);
                _isInitialised = true;

                return _fromMappingTable;
            }
        }
    }
}