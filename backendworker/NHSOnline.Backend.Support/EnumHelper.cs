using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace NHSOnline.Backend.Support
{
    public static class EnumHelper
    {
        private static ConcurrentDictionary<Type, ConcurrentDictionary<string, string>> DescriptionsCache { get; }

        private static readonly Type DescriptionAttributeType = typeof(DescriptionAttribute);

        static EnumHelper()
        {
            DescriptionsCache = new ConcurrentDictionary<Type, ConcurrentDictionary<string, string>>();
        }

        public static bool TryParseFromDescription<TEnum>(string description, out TEnum value)
            where TEnum : struct, IConvertible
        {
            var type = GetType<TEnum>();

            try
            {
                value = (TEnum) ParseFromDescriptionInternal(type, description);
                return true;
            }
            catch
            {
                value = default;
                return false;
            }
        }

        public static Type GetType<TEnum>()
        {
            var type = typeof(TEnum);
            if (!type.IsEnum)
                throw new ArgumentException("Type provided must be an Enum.");

            return type;
        }
        
        public static string GetDescriptionOrThrowException<TEnum>(TEnum enumerationValue)
        {
            var type = GetType<TEnum>();

            var success = GetDescriptions(type)
                .TryGetValue(enumerationValue.ToString(), out var enumDescription);


            if (success && !string.Equals(enumDescription, enumerationValue.ToString(), StringComparison.Ordinal))
            {
                return enumDescription;
            }

            throw new ArgumentException($"No Description attribute on enum {enumerationValue.ToString()}");
        }

        private static object ParseFromDescriptionInternal(Type type, string description)
        {
            if (description == null)
                throw new ArgumentNullException(nameof(description));

            if (string.IsNullOrWhiteSpace(description))
                throw new ArgumentException("Must specify valid information for parsing in the string.");

            var enumName = GetDescriptions(type)
                .Where(d => string.Equals(d.Value, description, StringComparison.Ordinal))
                .Select(d => d.Key)
                .FirstOrDefault();

            if (enumName == null)
                throw new ArgumentException($"Requested value '{description}' was not found.");

            return Enum.Parse(type, enumName);
        }

        private static IDictionary<string, string> GetDescriptions(Type type)
        {
            if (DescriptionsCache.TryGetValue(type, out var descriptions))
                return descriptions;

            var newDescriptions = new ConcurrentDictionary<string, string>();

            foreach (var fieldInfo in type.GetFields())
            {
                var attribute = (DescriptionAttribute) fieldInfo.GetCustomAttributes(DescriptionAttributeType, false)
                    .FirstOrDefault();

                newDescriptions.TryAdd(fieldInfo.Name, attribute != null ? attribute.Description : fieldInfo.Name);
            }

            DescriptionsCache.TryAdd(type, newDescriptions);

            return newDescriptions;
        }
    }
}