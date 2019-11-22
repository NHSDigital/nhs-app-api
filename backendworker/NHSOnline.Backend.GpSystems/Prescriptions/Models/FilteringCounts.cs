namespace NHSOnline.Backend.GpSystems.Prescriptions.Models
{
    public class FilteringCounts
    {
        public int ReceivedCount { get; set; }
            
        public int FilteredRemainingRepeatsCount { get; set; }
            
        public int FilteredMaxAllowanceDiscardedCount { get; set; }

        public int ReturnedCount { get; set; }
    }
}