using NHSOnline.Backend.Worker.GpSystems.Demographics.Models;
using NHSOnline.Backend.Worker.GpSystems.Demographics;
using NHSOnline.Backend.Worker.Support;

namespace NHSOnline.Backend.Worker.Areas.Demographics
{
    public class SuccessfulDemographicsResultMapper : IMapper<DemographicsResult.SuccessfullyRetrieved, SuccessfulDemographicsResult>
    {
        public SuccessfulDemographicsResult Map(DemographicsResult.SuccessfullyRetrieved source)
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