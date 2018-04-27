namespace NHSOnline.Backend.Worker.IntegrationTests.Worker.Models.Patient
{
    public class UserSessionRequest
    {
        public string AuthCode { get; set; }

        public string CodeVerifier { get; set; }
    }
}
