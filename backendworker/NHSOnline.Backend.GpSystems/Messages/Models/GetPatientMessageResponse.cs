namespace NHSOnline.Backend.GpSystems.Messages.Models
{
    public class GetPatientMessageResponse
    {
        public GetPatientMessageResponse()
        {
            MessageDetails = new PatientMessageDetails();
        }
        
        public PatientMessageDetails MessageDetails { get; }
    }
}