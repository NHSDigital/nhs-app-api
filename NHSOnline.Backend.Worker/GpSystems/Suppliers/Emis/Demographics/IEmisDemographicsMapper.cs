using NHSOnline.Backend.Worker.GpSystems.Suppliers.Emis.Models;

namespace NHSOnline.Backend.Worker.GpSystems.Suppliers.Emis.Demographics
{
    public interface IEmisDemographicsMapper
    {
        Areas.MyRecord.Models.DemographicsResponse Map(DemographicsGetResponse demographicsGetResponse);
    }
}