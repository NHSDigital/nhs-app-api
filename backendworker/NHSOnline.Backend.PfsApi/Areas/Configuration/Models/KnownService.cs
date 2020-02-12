namespace NHSOnline.Backend.PfsApi.Areas.Configuration.Models
{
    public abstract class KnownService
    {
        public bool AllowNativeInteraction { get; set; }
        
        public int MenuTab { get; set; }
        
        public bool OpenExternally { get; set; }
        
        public bool RequiresAssertedLoginIdentity { get; set; }
        
        public bool ValidateSession { get; set; }
    }
}