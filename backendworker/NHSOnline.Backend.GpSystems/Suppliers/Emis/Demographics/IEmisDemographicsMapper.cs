using NHSOnline.Backend.GpSystems.Demographics.Models;
using NHSOnline.Backend.GpSystems.Demographics;
using NHSOnline.Backend.GpSystems.Suppliers.Emis.Models;

namespace NHSOnline.Backend.GpSystems.Suppliers.Emis.Demographics
{
    public interface IEmisDemographicsMapper
    {
        DemographicsResponse Map(DemographicsGetResponse demographicsGetResponse);
    }
}