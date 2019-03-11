namespace NHSOnline.Backend.Worker.GpSearch.Models
{
    public class OpeningTime
    {
        public ResponseEnums.WeekDay? WeekDay { get; set; }

        public string Times { get; set; }

        public ResponseEnums.OpeningTimeType OpeningTimeType { get; set; }

        public bool IsOpen { get; set; }

        public string AdditionalOpeningDate { get; set; }
    }
}
