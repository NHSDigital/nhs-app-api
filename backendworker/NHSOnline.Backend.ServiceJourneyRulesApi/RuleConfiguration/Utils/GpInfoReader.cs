using System;
using System.Collections.Generic;
using System.Linq;
using CsvHelper;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.ServiceJourneyRulesApi.RuleConfiguration.Models;
using NHSOnline.Backend.ServiceJourneyRulesApi.RuleConfiguration.Utils.Converters;

namespace NHSOnline.Backend.ServiceJourneyRulesApi.RuleConfiguration.Utils
{
    internal class GpInfoReader : IGpInfoReader
    {
        private readonly ILogger _logger;
        private readonly IProcessState _processState;
        private readonly IFileHandler _fileHandler;
        
        private const string ErrorReadingMessage = "Error reading the GP info csv file";

        public GpInfoReader(ILogger<GpInfoReader> logger, IFileHandler fileHandler, IProcessState processState)
        {
            _logger = logger;
            _fileHandler = fileHandler;
            _processState = processState;
        }
        
        public IDictionary<string, GpInfo> GetGpInfo(string filePath)
        {
            try
            {
                var result = ReadGpInfoFromCsv(filePath).ToList();

                if (!_processState.HasError)
                {
                    return result.GroupBy(g => g.Ods)
                        .Select(g => g.OrderByDescending(i => i.EndpointCreated).First())
                        .ToDictionary(g => g.Ods, g => g);
                }

                _logger.LogError(ErrorReadingMessage);
            }
            catch (Exception e)
            {
                _logger.LogError(e, ErrorReadingMessage);
            }
            
            return null;
        }
        
        private IEnumerable<GpInfo> ReadGpInfoFromCsv(string filePath)
        {
            var reader = _fileHandler.GetTextReaderToReadFileContent(filePath);
            
            var csvReader = new CsvReader(reader);
            
            csvReader.Configuration.TypeConverterCache.AddConverter<GpInfoSupplier>(new EnumDescriptionConverter<GpInfoSupplier>(_processState, _logger));
            
            csvReader.Configuration.PrepareHeaderForMatch = (header, index) => header.ToUpperInvariant();
            
            return csvReader.GetRecords<GpInfo>();
        }
    }
}