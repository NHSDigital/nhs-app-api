using NHSOnline.Backend.Worker.Areas.Demographics.Models;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Vision.Models.PatientRecord;

namespace NHSOnline.Backend.Worker.GpSystems.Suppliers.Vision.Demographics
{
    public interface IVisionDemographicsMapper
    {
        DemographicsResponse Map(VisionDemographics patientDemographics, string nhsNumber);
    }
}