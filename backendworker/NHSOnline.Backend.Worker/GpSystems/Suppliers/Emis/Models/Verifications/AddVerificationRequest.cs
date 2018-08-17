namespace NHSOnline.Backend.Worker.GpSystems.Suppliers.Emis.Models.Verifications
{
    public class AddVerificationRequest
    {
        public string NationalPracticeCode { get; set; }

        public string NhsNumber { get; set; }

        public string Token { get; set; }

        public string AdditionalComment { get; set; }
    }
}
