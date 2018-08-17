namespace NHSOnline.Backend.Worker.GpSystems.Suppliers.Emis.Models
{
    public class AddNhsUserRequest
    {
        public string NationalPracticeCode { get; set; }

        public string NhsNumber { get; set; }

        public string EmailAddress { get; set; }
    }
}
