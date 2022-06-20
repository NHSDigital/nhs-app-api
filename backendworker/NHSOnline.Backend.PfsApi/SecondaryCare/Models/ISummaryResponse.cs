namespace NHSOnline.Backend.PfsApi.SecondaryCare.Models
{
    public interface ISummaryResponse
    {
        int AppointmentCount { get; set; }
        int ReferralCount { get; set; }

        void Sort();
    }
}