using NHSOnline.Backend.ServiceJourneyRulesApi.RuleConfiguration.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace NHSOnline.Backend.ServiceJourneyRulesApi.RuleConfiguration.Utils
{
    internal class ConfigurationExporter : IConfigurationExporter
    {
        private readonly List<string> ExcludedProperties = new List<string>
        {
            nameof(TargetConfiguration.Schema)
        };

        private readonly IServiceJourneyRulesConfiguration _serviceJourneyRulesConfiguration;
        private readonly IYamlReaderFactory _yamlReaderFactory;
        private readonly IFileHandler _fileHandler;
        private readonly ILogger _logger;

        public ConfigurationExporter(
            IServiceJourneyRulesConfiguration serviceJourneyRulesConfiguration,
            IYamlReaderFactory yamlReaderFactory,
            IFileHandler fileHandler,
            ILogger<ConfigurationExporter> logger)
        {
            _serviceJourneyRulesConfiguration = serviceJourneyRulesConfiguration;
            _yamlReaderFactory = yamlReaderFactory;
            _fileHandler = fileHandler;
            _logger = logger;
        }

        public async Task<int> Export()
        {
            var outputFolderPath = _serviceJourneyRulesConfiguration.OutputFolderPath;
            var files = _fileHandler.GetFiles(outputFolderPath);
            if (!files.Any())
            {
                _logger.LogInformation($"No files to process at {outputFolderPath}");
                return 0;
            }

            var propertyNames = new List<string>();
            PopulatePropertyNames(typeof(TargetConfiguration), propertyNames);
            var headerLine = string.Join(",", propertyNames);
            var outputLines = new List<string> { headerLine };

            var context = new LoadContext();
            _fileHandler.ReadEmbeddedResourceFromFileName(Constants.FileNames.JourneyConfigurationSchema, out var data);

            context.TargetSchema = data;
            foreach (var filePath in files)
            {
                var reader = _yamlReaderFactory.GetReader<TargetConfiguration>(filePath, context.TargetSchema);
                var targetConfiguration = await reader.GetData();
                if (targetConfiguration != null)
                {
                    outputLines.Add(ToCsv(targetConfiguration, propertyNames));
                }
            }

            await using var textWriter = _fileHandler.GetTextWriter(_serviceJourneyRulesConfiguration.CsvExportOutputFilePath);
            outputLines.ForEach(line => textWriter.WriteLine(line));

            return 0;
        }

        private void PopulatePropertyNames(Type type, IList<string> propertyNames, string prefix = "")
        {
            foreach (var prop in type.GetProperties())
            {
                if (ExcludedProperties.Contains($"{prefix}{prop.Name}"))
                {
                    continue;
                }

                if (IsSimpleType(prop.PropertyType))
                {
                    propertyNames.Add($"{prefix}{prop.Name}");
                }
                else
                {
                    PopulatePropertyNames(prop.PropertyType, propertyNames, $"{prefix}{prop.Name}.");
                }
            }
        }

        private string ToCsv<T>(T obj, IEnumerable<string> propertyNames, string separator = ",")
        {
            var values = new List<string>();

            foreach (var propertyName in propertyNames)
            {
                values.Add(GetPropertyValue(obj, propertyName)?.ToString());
            }

            return string.Join(separator, values.Select(x => x));
        }

        private object GetPropertyValue(object obj, string propName)
        {
            try
            {
                if (obj == null)
                {
                    return null;
                }

                if (propName.Contains(".", StringComparison.Ordinal))
                {
                    var temp = propName.Split('.', 2);
                    return GetPropertyValue(GetPropertyValue(obj, temp[0]), temp[1]);
                }

                var type = obj.GetType();
                var prop = type.GetProperty(propName);

                if (prop == null)
                {
                    return null;
                }

                var value = prop.GetValue(obj, null);

                if (value != null && IsCollection(prop.PropertyType))
                {
                    var collectionValues = new List<string>();
                    foreach (var item in value as IEnumerable)
                    {
                        if (item.GetType().IsClass)
                        {
                            // Commas are the csv delimiter so can't have commas here.
                            collectionValues.Add(
                                JsonConvert.SerializeObject(item).Replace(",", "|", StringComparison.Ordinal));
                        }
                        else
                        {
                            collectionValues.Add(item.ToString());
                        }
                    }

                    return collectionValues.Any() ? string.Join("; ", collectionValues) : string.Empty;
                }

                return value;
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Error getting or writing value for {propName}");
                throw;
            }
        }

        private static bool IsSimpleType(Type type)
        {
            return type == typeof(string)
                   || IsBool(type)
                   || IsEnum(type)
                   || IsCollection(type);
        }

        private static bool IsBool(Type type)
        {
            return type == typeof(bool)
                   || type == typeof(bool?);
        }

        private static bool IsEnum(Type type)
        {
            return type == typeof(Enum)
                   || Nullable.GetUnderlyingType(type)?.IsEnum == true;
        }

        private static bool IsCollection(Type type)
        {
            if (type == null || type == typeof(string))
            {
                return false;
            }
            return typeof(IEnumerable).IsAssignableFrom(type);
        }
    }
}
