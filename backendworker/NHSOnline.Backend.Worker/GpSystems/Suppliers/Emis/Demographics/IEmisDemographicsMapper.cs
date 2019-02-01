using NHSOnline.Backend.Worker.GpSystems.Demographics.Models;
using NHSOnline.Backend.Worker.GpSystems.Demographics;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Emis.Models;

namespace NHSOnline.Backend.Worker.GpSystems.Suppliers.Emis.Demographics
{
    public interface IEmisDemographicsMapper
    {
        DemographicsResponse Map(DemographicsGetResponse demographicsGetResponse);
    }
}