namespace NHSOnline.Backend.Worker.Areas.MyRecord.Models
{
    public class AllergyItem
    {
        public string Name { get; set; }
        public MyRecordDate Date { get; set; }
        public string Drug { get; set; }
        public string Reaction { get; set; }
    }
}