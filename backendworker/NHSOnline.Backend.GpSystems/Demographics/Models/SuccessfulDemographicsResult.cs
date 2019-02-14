namespace NHSOnline.Backend.GpSystems.Demographics.Models
{
    public class SuccessfulDemographicsResult
    {
        public SuccessfulDemographicsResponse Response { get; }
        
        public SuccessfulDemographicsResult(SuccessfulDemographicsResponse response)
        {
            Response = response;     
        }
    }
}