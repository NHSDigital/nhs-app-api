using System.Threading.Tasks;

namespace NHSOnline.Backend.Metrics
{
    public interface IMetricLogger
    {
        Task Login(LoginData data);

        Task UpliftStarted();

        Task UserResearchOptOut();

        Task UserResearchOptIn();

        Task TermsAndConditionsInitialConsent();

        Task MessageRead(MessageReadData data);

        Task NotificationsEnabled();

        Task NotificationsDisabled();
    }
}
