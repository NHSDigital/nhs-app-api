using NHSOnline.Backend.GpSystems.Linkage.Models;

namespace NHSOnline.Backend.GpSystems.Linkage
{
    public interface ILinkageRequestValidationService
    {
        bool Validate(GetLinkageRequest request);
        
        bool Validate(CreateLinkageRequest request);
    }
}