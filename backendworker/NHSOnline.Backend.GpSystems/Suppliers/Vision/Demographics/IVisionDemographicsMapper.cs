using NHSOnline.Backend.GpSystems.Demographics;
using NHSOnline.Backend.GpSystems.Suppliers.Vision.Models.PatientRecord;

namespace NHSOnline.Backend.GpSystems.Suppliers.Vision.Demographics
{
    public interface IVisionDemographicsMapper
    {
        DemographicsResponse Map(VisionDemographics patientDemographics, string nhsNumber);
    }
}