namespace NHSOnline.Backend.GpSystems.Suppliers.Emis.Models
{
    public class PracticeSettingsGetResponse
    {
        public PracticeSettingsMessages Messages { get; set; }
        
        public PracticeSettingsInputRequirements InputRequirements { get; set; }
        
        public PracticeSettingsServices Services { get; set; }
    }
}
