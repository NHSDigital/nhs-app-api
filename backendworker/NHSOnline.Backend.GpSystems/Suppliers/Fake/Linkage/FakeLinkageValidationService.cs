using NHSOnline.Backend.GpSystems.Linkage;
using NHSOnline.Backend.GpSystems.Linkage.Models;

namespace NHSOnline.Backend.GpSystems.Suppliers.Fake.Linkage
{
    public class FakeLinkageValidationService : ILinkageValidationService
    {
        public bool IsGetValid(GetLinkageRequest request)
        {
            return true;
        }

        public bool IsPostValid(CreateLinkageRequest request)
        {
            return true;
        }
    }
}