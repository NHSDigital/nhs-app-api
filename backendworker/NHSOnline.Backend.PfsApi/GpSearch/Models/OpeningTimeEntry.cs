namespace NHSOnline.Backend.PfsApi.GpSearch.Models
{
    public class OpeningTimeEntry
    {
        public ResponseEnums.WeekDay? Weekday { get; set; }

        public ResponseEnums.OpeningTimeType OpeningTimeType { get; set; }

        public bool IsOpen { get; set; }

        public string AdditionalOpeningDate { get; set; }

        public string OpeningTime { get; set; }

        public string ClosingTime { get; set; }
    }
}
