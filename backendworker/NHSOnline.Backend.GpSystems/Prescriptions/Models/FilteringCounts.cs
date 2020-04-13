namespace NHSOnline.Backend.GpSystems.Prescriptions.Models
{
    public class FilteringCounts
    {
        public int ReceivedCount { get; set; }
            
        public int ReceivedRepeatsCount { get; set; }
            
        public int ExcessRepeatsCount { get; set; }

        public int ReturnedCount { get; set; }
    }
}