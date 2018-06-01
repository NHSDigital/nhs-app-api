using NHSOnline.Backend.Worker.Bridges.Emis.Models;

namespace NHSOnline.Backend.Worker.Bridges.Emis.Demographics
{
    public interface IEmisDemographicsMapper
    {
        Areas.MyRecord.Models.DemographicsResponse Map(DemographicsGetResponse demographicsGetResponse);
    }
}