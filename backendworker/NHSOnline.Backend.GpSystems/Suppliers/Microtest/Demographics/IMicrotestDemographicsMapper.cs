using NHSOnline.Backend.GpSystems.Demographics;
using NHSOnline.Backend.GpSystems.Suppliers.Microtest.Models.Demographics;

namespace NHSOnline.Backend.GpSystems.Suppliers.Microtest.Demographics
{
    public interface IMicrotestDemographicsMapper
    {
        DemographicsResponse Map(DemographicsGetResponse demographicsGetResponse);
    }
}