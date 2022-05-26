using System;
using System.Diagnostics;
using System.Threading.Tasks;
using NHSOnline.Backend.Auditing;
using NHSOnline.Backend.Support;
using NHSOnline.Backend.Users.Repository;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Auth.CitizenId.Models;
using NHSOnline.Backend.Users.Areas.Devices.Models;

namespace NHSOnline.Backend.Users.Registrations
{
    public class NotificationsDecisionAuditService : INotificationsDecisionAuditService
    {
        private const string NotificationToggleResponseAuditType = AuditingOperations.NotificationToggleResponse;
        private const string NotificationPromptResponseAuditType = AuditingOperations.InitialNotificationPromptDecision;
        private readonly ILogger<NotificationsDecisionAuditService> _logger;
        private readonly IAuditor _auditor;
        private string _auditType;
        private string _auditDetail;
        private string _notificationsDecision;
        private AccessToken _accessToken;

        public NotificationsDecisionAuditService(ILogger<NotificationsDecisionAuditService> logger, IAuditor auditor)
        {
            _logger = logger;
            _auditor = auditor;
        }

        public async Task LogAudit(NotificationsAuditData notificationsAuditData,
            AccessToken accessToken)
        {
            if (notificationsAuditData.NotificationsRegistered)
            {
                await AuditNotificationsEnabled(
                    notificationsAuditData.NotificationsDecisionSource,
                    accessToken);
            }
            else
            {
                await AuditNotificationsDisabled(
                    notificationsAuditData.NotificationsDecisionSource,
                    accessToken);
            }
        }

        private async Task AuditNotificationsDecision()
        {
            try
            {
                await _auditor.PostOperationAuditSessionEvent(_accessToken.ToString(),
                    string.IsNullOrEmpty(_accessToken.NhsNumber) ? " " : _accessToken.NhsNumber,
                    Supplier.Unknown,
                    _auditType,
                    _auditDetail,
                    null,
                    null);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Exception thrown auditing {AuditType} {Result}", _auditType, nameof(RegisterDeviceResult.Created));
            }
        }

        private async Task SetupNotificationsDecisionAudit(NotificationsDecisionSource notificationsDecisionSource)
        {
            switch (notificationsDecisionSource) {
                case NotificationsDecisionSource.Toggle:
                    _auditType = NotificationToggleResponseAuditType;
                    _auditDetail = $"Notification toggled. optIn={_notificationsDecision}";
                    break;
                case NotificationsDecisionSource.Prompt:
                    _auditType = NotificationPromptResponseAuditType;
                    _auditDetail = $"Initial notification prompt decision made. optIn={_notificationsDecision}";
                    break;
                default:
                    _auditType = NotificationToggleResponseAuditType;
                    _auditDetail = $"Unknown Source Notification Decision. optIn={_notificationsDecision}";
                    break;
            }

            await Task.CompletedTask;
        }

        private async Task AuditNotificationsEnabled(NotificationsDecisionSource notificationsDecisionSource,
            AccessToken accessToken)
        {
            _notificationsDecision = "true";
            _accessToken = accessToken;
            await SetupNotificationsDecisionAudit(notificationsDecisionSource);
            await AuditNotificationsDecision();
        }

        private async Task AuditNotificationsDisabled(NotificationsDecisionSource notificationsDecisionSource,
            AccessToken accessToken)
        {
            _notificationsDecision = "false";
            _accessToken = accessToken;
            await SetupNotificationsDecisionAudit(notificationsDecisionSource);
            await AuditNotificationsDecision();
        }
    }
}