namespace NHSOnline.Backend.PfsApi.SecondaryCare.Models
{
    public enum SummaryItemType
    {
        UpcomingAppointment,
        Referral,
    }

    public abstract class SecondaryCareSummaryItem
    {
        public abstract string ItemType { get; }
    }
}