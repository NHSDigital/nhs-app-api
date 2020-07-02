using System;

namespace NHSOnline.Backend.GpSystems.Suppliers.Fake
{
    [AttributeUsage(AttributeTargets.Interface)]
    public sealed class FakeGpAreaAttribute : Attribute
    {
        public string AreaName { get; }

        public FakeGpAreaAttribute(string areaName)
        {
            AreaName = areaName;
        }
    }
}
