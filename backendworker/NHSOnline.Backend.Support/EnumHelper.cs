using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace NHSOnline.Backend.Support
{
    public static class EnumHelper
    {
        private static Dictionary<Type, Dictionary<string, string>> DescriptionsCache { get; }

        private static readonly Type DescriptionAttributeType = typeof(DescriptionAttribute);

        static EnumHelper()
        {
            DescriptionsCache = new Dictionary<Type, Dictionary<string, string>>();
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

        public static TEnum ParseFromDescription<TEnum>(string description)
            where TEnum : struct, IConvertible
        {
            var type = GetType<TEnum>();

            return (TEnum) ParseFromDescriptionInternal(type, description);
        }

        public static Type GetType<TEnum>()
        {
            var type = typeof(TEnum);
            if (!type.IsEnum)
                throw new ArgumentException("Type provided must be an Enum.");

            return type;
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

        private static Dictionary<string, string> GetDescriptions(Type type)
        {
            if (DescriptionsCache.TryGetValue(type, out var descriptions))
                return descriptions;

            descriptions = new Dictionary<string, string>();

            foreach (var fieldInfo in type.GetFields())
            {
                var attribute = (DescriptionAttribute) fieldInfo.GetCustomAttributes(DescriptionAttributeType, false)
                    .FirstOrDefault();

                descriptions.Add(fieldInfo.Name, attribute != null ? attribute.Description : fieldInfo.Name);
            }

            DescriptionsCache.Add(type, descriptions);

            return descriptions;
        }
    }
}