using NHSOnline.Backend.Worker.Areas.Linkage.Models;

namespace NHSOnline.Backend.Worker.GpSystems.Linkage
{
    public interface ILinkageRequestValidationService
    {
        bool Validate(GetLinkageRequest request);
        
        bool Validate(CreateLinkageRequest request);
    }
}