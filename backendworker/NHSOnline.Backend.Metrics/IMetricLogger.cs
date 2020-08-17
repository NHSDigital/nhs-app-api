using System.Threading.Tasks;

namespace NHSOnline.Backend.Metrics
{
    public interface IMetricLogger
    {
        Task Login();

        Task UpliftStarted();

        Task UserResearchOptOut();

        Task UserResearchOptIn();

        Task TermsAndConditionsInitialConsent();

        Task MessageRead();

        Task NotificationsEnabled();

        Task NotificationsDisabled();
    }
}
