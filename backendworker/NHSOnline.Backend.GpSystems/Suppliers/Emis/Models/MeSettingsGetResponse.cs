namespace NHSOnline.Backend.GpSystems.Suppliers.Emis.Models
{
    public class MeSettingsGetResponse
    {
        public string ApplicationLinkLevel { get; set; }

        public string AccountRegistrationStatus { get; set; }

        public UserGpPracticeSettings AssignedServices { get; set; }

        public UserGpPracticeSettings EffectiveServices { get; set; }
    }
}
