namespace NHSOnline.Backend.Worker.Mocking.Emis.Models
{
    public class EndUserSessionResponse
    {
        public string EndUserSessionId { get; }

        public EndUserSessionResponse(string endUserSessionId)
        {
            EndUserSessionId = endUserSessionId;
        }
    }
}
