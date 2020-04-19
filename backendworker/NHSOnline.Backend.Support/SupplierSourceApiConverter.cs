using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace NHSOnline.Backend.Support
{
    internal sealed class SupplierSourceApiConverter
    {
        private static readonly Dictionary<Supplier, SourceApi> SupplierToSourceApiMap = LoadSupplierToSourceApiMap();

        public static SupplierSourceApiConverter Instance { get; } = new SupplierSourceApiConverter();

        public SourceApi this[Supplier supplier]
        {
            get
            {
                if (SupplierToSourceApiMap.TryGetValue(supplier, out SourceApi sourceApi))
                {
                    return sourceApi;
                }

                return SourceApi.None;
            }
        }

        private static Dictionary<Supplier, SourceApi> LoadSupplierToSourceApiMap()
            => typeof(Supplier)
                .GetFields()
                .Select(f => (f.Name, CustomAttributeExtensions.GetCustomAttribute<SourceApiAttribute>((MemberInfo) f)?.SourceApi))
                .Where(t => t.SourceApi != null)
                .ToDictionary(t => (Supplier) Enum.Parse(typeof(Supplier), t.Name), t => t.SourceApi.Value);
    }
}