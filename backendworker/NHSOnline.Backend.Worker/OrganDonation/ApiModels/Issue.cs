namespace NHSOnline.Backend.Worker.OrganDonation.ApiModels
{
    public class Issue
    {
        public string Code { get; set; }

        public CodeableConcept Details { get; set; }

        public string Diagnostics { get; set; }
    }
}