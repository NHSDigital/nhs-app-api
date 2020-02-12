namespace NHSOnline.Backend.PfsApi.Areas.Configuration.Models
{
    public class SubService: KnownService
    {
        public string Path { get; set; }
        
        public string QueryString { get; set; }
    }
}