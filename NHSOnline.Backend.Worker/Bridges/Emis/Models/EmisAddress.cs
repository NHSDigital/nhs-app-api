namespace NHSOnline.Backend.Worker.Bridges.Emis.Models
{
    public class EmisAddress
    {
        public string HouseNameFlatNumber { get; set; }
        public string NumberStreet { get; set; }
        public string Village { get; set; }
        public string Town { get; set; }
        public string County { get; set; }
        public string Postcode { get; set; }
    }
}