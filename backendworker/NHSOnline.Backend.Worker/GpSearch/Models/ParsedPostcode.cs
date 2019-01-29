namespace NHSOnline.Backend.Worker.GpSearch.Models
{
    public class ParsedPostcode
    {
        public bool IsPostcode { get; set; }
        public string Inward { get; set; }
        public string Postcode { get; set; }
    }
}