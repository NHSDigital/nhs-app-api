using System;
using System.Linq;
using System.Reflection;

namespace NHSOnline.Backend.Support
{
    public static class TypeExtensions
    {
        public static bool MatchesNamespacePrefix(this Type type, string namespacePrefix)
        {
            return type?.Namespace?.StartsWith(
                namespacePrefix,
                StringComparison.InvariantCulture
            ) ?? false;
        }

        public static bool HasAttribute<T>(this MemberInfo memberInfo) where T : Attribute
        {
            return memberInfo?.GetCustomAttributes<T>().Any() ?? false;
        }

        public static T GetAttribute<T>(this MemberInfo memberInfo) where T : Attribute
        {
            return memberInfo?.GetCustomAttributes<T>().FirstOrDefault();
        }
    }
}
