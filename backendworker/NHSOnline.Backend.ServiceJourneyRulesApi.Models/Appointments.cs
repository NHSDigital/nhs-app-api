namespace NHSOnline.Backend.ServiceJourneyRulesApi.Models
{
    public class Appointments : Journey<AppointmentsProvider>
    {
        public string InformaticaUrl { get; set; }
    }
}