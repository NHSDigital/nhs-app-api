namespace NHSOnline.Backend.Worker.OrganDonation.Models
{
    public class Issue
    {
        public string Code { get; set; }

        public CodeableConcept Details { get; set; }

        public string Diagnostics { get; set; }
    }
}