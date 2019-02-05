using System;
using NHSOnline.Backend.Worker.OrganDonation;
using NHSOnline.Backend.Worker.Support.Auditing;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace NHSOnline.Backend.Worker.Areas.OrganDonation
{
    public class OrganDonationRegistrationAuditingVisitor : IOrganDonationRegistrationResultVisitor<Task>
    {
        private readonly IAuditor _auditor;
        private readonly ILogger<OrganDonationController> _logger;
        
        private const string AuditType = Constants.AuditingTitles.OrganDonationRegistrationAuditTypeResponse;

        public OrganDonationRegistrationAuditingVisitor(IAuditor auditor, ILogger<OrganDonationController> logger)
        {
            _auditor = auditor;
            _logger = logger;
        }
        
        public async Task Visit(OrganDonationRegistrationResult.SuccessfullyRegistered result)
        {
            try
            {
                await _auditor.Audit(AuditType, "The organ donation decision has been successfully registered");
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Exception thrown auditing {AuditType} {nameof(OrganDonationRegistrationResult.SuccessfullyRegistered)}");
            }
        }
        
        public async Task Visit(OrganDonationRegistrationResult.SystemError result)
        {
            try
            {
                await _auditor.Audit(AuditType, "There was an issue registering the organ donation decision");
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Exception thrown auditing {AuditType} {nameof(OrganDonationRegistrationResult.SystemError)}");
            }
        }

        public async Task Visit(OrganDonationRegistrationResult.UpstreamError result)
        {
            try
            {
                await _auditor.Audit(AuditType, "There was an upstream error when registering the organ donation decision");
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Exception thrown auditing {AuditType} {nameof(OrganDonationRegistrationResult.UpstreamError)}");
            }
        }

        public async Task Visit(OrganDonationRegistrationResult.Timeout result)
        {
            try
            {
                await _auditor.Audit(AuditType, "The organ donation registration system took too long to respond");
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Exception thrown auditing {AuditType} {nameof(OrganDonationRegistrationResult.Timeout)}");
            }
        }
    }
}
