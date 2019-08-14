using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Support.Logging;
using NHSOnline.Backend.UsersApi.Areas.Devices.Models;

namespace NHSOnline.Backend.UsersApi.Notifications
{
    public class NotificationRegistrationService : INotificationRegistrationService
    {
        private readonly ILogger<NotificationRegistrationService> _logger;
        private readonly INotificationService _notificationService;

        public NotificationRegistrationService(INotificationService notificationService, ILogger<NotificationRegistrationService> logger)
        {
            _notificationService = notificationService;
            _logger = logger;
        }

        public async Task<RegistrationResult> Register(RegisterDeviceRequest request, string nhsLoginId)
        {
            try
            {
                _logger.LogEnter();
                var registrationRequest = CreateRegistrationRequest(request, nhsLoginId);
            
                return await _notificationService.Register(registrationRequest);
            }
            finally
            {
                _logger.LogExit();
            }
        }
        
        private NotificationRegistrationRequest CreateRegistrationRequest(RegisterDeviceRequest request, string nhsLoginId)
        {
            return new NotificationRegistrationRequest
            {
                DevicePns = request.DevicePns,
                DeviceType = request.DeviceType,
                NhsLoginId = nhsLoginId
            };
        }
    }
}