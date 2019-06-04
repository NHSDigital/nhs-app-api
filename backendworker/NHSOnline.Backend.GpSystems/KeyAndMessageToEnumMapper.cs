using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;

namespace NHSOnline.Backend.GpSystems
{
    public class KeyAndMessageToEnumMapper<TEnum> where TEnum:struct
    {
        private ILogger _logger;

        private readonly List<Mapping> _keyAndMessageMappings = new List<Mapping>();
        private readonly Dictionary<string, TEnum> _keyToEnumMappers = new Dictionary<string, TEnum> ();
        
        public KeyAndMessageToEnumMapper<TEnum> AddKeyToEnum(string message, TEnum mapping)
        {
            _keyToEnumMappers.Add(message, mapping);
            return this;
        }

        public KeyAndMessageToEnumMapper<TEnum> Add(string key, string message, TEnum mapping)
        {
            _keyAndMessageMappings.Add(new Mapping(key, message, mapping));
            return this;
        }

        public TEnum? Map(ILogger logger, string key, params string[] messages)
        {
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }

            _logger = logger;
            var mappings = messages.Select(message => Map(key, message?.Trim()))
                .Where(mapping => mapping != null).ToList();

            return mappings.Any() ? mappings.First() : Map(key, null);
        }

        private TEnum? Map(string key, string message)
        {
            var keyAndMessageMapping = MapKeyAndMessageToEnum(key, message);

            if (keyAndMessageMapping != null)
            {
                return keyAndMessageMapping.Value;
            }

            var successfulKeyMapping = _keyToEnumMappers.TryGetValue(key, out var keyMapping);

            _logger.LogInformation(successfulKeyMapping
                ? $"Mapping found with Key '{key}'"
                : $"No mapping found with Key '{key}'");
            if (successfulKeyMapping)
            {
                return keyMapping;
            }

            if (message != null)
            {
                var successfulMessageMapping =
                    _keyToEnumMappers.TryGetValue(message, out var messageMapping);
                _logger.LogInformation(successfulMessageMapping
                    ? $"Mapping found with Message '{message}'"
                    : $"No mapping found with Message '{message}'");
                if (successfulMessageMapping)
                {
                    return messageMapping;
                }
            }

            return null;
        }

        private TEnum? MapKeyAndMessageToEnum(string key, string message)
        {
            var foundMapping = _keyAndMessageMappings
                .FirstOrDefault(mapping =>
                    KeyMatches(mapping, key) &&
                    MessageMatches(mapping, message));

            _logger.LogInformation(foundMapping != null
                ? $"Mapping found with Key '{key}' and Message '{message}'"
                : $"No mapping found with Key '{key}' and Message '{message}'");

            return foundMapping?.MappedValue;
        }

        private static bool KeyMatches(Mapping mapping, string key)
        {
            return string.Equals(mapping.Key, key, StringComparison.OrdinalIgnoreCase);
        }

        private static bool MessageMatches(Mapping mapping, string message)
        {
            if (!string.IsNullOrWhiteSpace(message))
            {
                return message.Contains(mapping.Message, StringComparison.OrdinalIgnoreCase);
            }

            return string.IsNullOrWhiteSpace(message) && string.IsNullOrWhiteSpace(mapping?.Message);
        }

        private class Mapping
        {
            public Mapping(string key, string message, TEnum mapping)
            {
                Key = key;
                Message = message;
                MappedValue = mapping;
            }

            public string Key { get;  }
            public string Message { get;  }
            public TEnum MappedValue { get; }
        }
    }
}
