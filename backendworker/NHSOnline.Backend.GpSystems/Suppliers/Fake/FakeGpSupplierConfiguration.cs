using System.Collections.Generic;

namespace NHSOnline.Backend.GpSystems.Suppliers.Fake.Users
{
    public class FakeGpSupplierConfiguration
    {
        public IDictionary<string, IDictionary<string, FakeUser>> DefaultUsers { get; set; }
    }
}
