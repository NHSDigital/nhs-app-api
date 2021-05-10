using System;
using System.Linq;

namespace NHSOnline.Backend.ServiceJourneyRulesApi.Models.Attributes
{
    public static class AttributeExtensions
    {
        public static T GetAttribute<T>(this Enum value) where T : Attribute
        {
            var type = value.GetType();
            var field = type.GetField(value.ToString());

            if (field == null)
            {
                return null;
            }

            return field
                .GetCustomAttributes(inherit: true)
                .OfType<T>()
                .FirstOrDefault();
        }

        public static bool HasAttribute<T>(this Enum value) where T : Attribute
        {
            return value.GetAttribute<T>() != null;
        }
    }
}
