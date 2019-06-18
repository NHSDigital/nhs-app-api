using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.TypeConversion;
using System;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Support;

namespace NHSOnline.Backend.ServiceJourneyRulesApi.RuleConfiguration.Utils.Converters
{
    internal class EnumDescriptionConverter<TEnum> : ITypeConverter
        where TEnum: struct, IConvertible
    {
        private readonly Type _enumType;
        private readonly IProcessState _processState;
        private readonly ILogger _logger;
        
        public EnumDescriptionConverter(IProcessState processState, ILogger logger)
        {
            _enumType = EnumHelper.GetType<TEnum>();
            _processState = processState;
            _logger = logger;
        }

        public string ConvertToString(object value, IWriterRow row, MemberMapData memberMapData)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Converts the string to an object.
        /// </summary>
        /// <param name="text">The string to convert to an object.</param>
        /// <param name="row">The <see cref="IReaderRow"/> for the current record.</param>
        /// <param name="memberMapData">The <see cref="MemberMapData"/> for the member being created.</param>
        /// <returns>The object created from the string.</returns>
        public object ConvertFromString(string text, IReaderRow row, MemberMapData memberMapData )
        {
            try
            {
                if (EnumHelper.TryParseFromDescription<TEnum>(text, out var value))
                {
                    return value;
                }
            }
            catch (ArgumentException e)
            {
                _logger.LogError(e.Message);
            }
            
            _logger.LogError($"{text} cannot be found in {_enumType.FullName} description/name");
            _processState.HasError = true;
            return default(TEnum);
        }
    }     
}

