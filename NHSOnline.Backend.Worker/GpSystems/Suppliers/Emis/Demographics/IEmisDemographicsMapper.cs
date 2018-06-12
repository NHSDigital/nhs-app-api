using NHSOnline.Backend.Worker.GpSystems.Suppliers.Emis.Models;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Emis.Models.Demographics;

namespace NHSOnline.Backend.Worker.GpSystems.Suppliers.Emis.Demographics
{
    public interface IEmisDemographicsMapper
    {
        DemographicsResponse Map(DemographicsGetResponse demographicsGetResponse);
    }
}