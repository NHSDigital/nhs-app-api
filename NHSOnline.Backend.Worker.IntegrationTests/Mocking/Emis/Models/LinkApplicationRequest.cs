namespace NHSOnline.Backend.Worker.IntegrationTests.Mocking.Emis.Models
{
    public class LinkApplicationRequest
    {
        public string Surname { get; set; }
        public string DateOfBirth { get; set; }
        public LinkageDetails LinkageDetails { get; set; }
    }
}
