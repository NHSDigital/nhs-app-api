using NHSOnline.Backend.GpSystems.Demographics.Models;
using NHSOnline.Backend.GpSystems.Demographics;
using NHSOnline.Backend.Support;

namespace NHSOnline.Backend.PfsApi.Areas.Demographics
{
    public class SuccessfulDemographicsResultMapper : IMapper<DemographicsResult.Success, SuccessfulDemographicsResult>
    {
        public SuccessfulDemographicsResult Map(DemographicsResult.Success source)
        {
            return new SuccessfulDemographicsResult(new SuccessfulDemographicsResponse
            {
                Address = source.Response.Address,
                Sex = source.Response.Sex,
                DateOfBirth = source.Response.DateOfBirth,
                NhsNumber = source.Response.NhsNumber,
                PatientName = source.Response.PatientName
            });
        }
    }
}