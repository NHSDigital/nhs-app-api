using System.Threading.Tasks;
using NHSOnline.Backend.Auditing;
using NHSOnline.Backend.Metrics;
using NHSOnline.Backend.PfsApi.OrganDonation;
using NHSOnline.Backend.Support.Session;

namespace NHSOnline.Backend.PfsApi.Areas.OrganDonation
{
    public class OrganDonationRegistrationUpdateAuditingVisitor : IOrganDonationRegistrationResultVisitor<Task>
    {
        private readonly IAuditor _auditor;
        private readonly IMetricLogger _metricLogger;
        private readonly P9UserSession _userSession;
        private const string AuditType = AuditingOperations.OrganDonationUpdateAuditTypeResponse;

        public OrganDonationRegistrationUpdateAuditingVisitor(IAuditor auditor, IMetricLogger metricLogger,
            P9UserSession userSession)
        {
            _auditor = auditor;
            _metricLogger = metricLogger;
            _userSession = userSession;
        }
        
        public async Task Visit(OrganDonationRegistrationResult.SuccessfullyRegistered result)
        {
            await _auditor.Audit(AuditType, "The organ donation decision has been successfully updated");
            await _metricLogger.OrganDonationUpdateRegistration(new OrganDonationData(_userSession.Key));
        }
        
        public async Task Visit(OrganDonationRegistrationResult.SystemError result)
        {
            await _auditor.Audit(AuditType, "There was an issue registering the organ donation decision update");
        }

        public async Task Visit(OrganDonationRegistrationResult.UpstreamError result)
        {
            await _auditor.Audit(AuditType, "There was an upstream error when registering the organ donation decision update");
        }

        public async Task Visit(OrganDonationRegistrationResult.Timeout result)
        {
            await _auditor.Audit(AuditType, "The organ donation registration update system took too long to respond");
        }
        
        public async Task Visit(OrganDonationRegistrationResult.BadRequest result)
        {
            await _auditor.Audit(AuditType, "The organ donation update registration request failed validation");
        }
    }
}
