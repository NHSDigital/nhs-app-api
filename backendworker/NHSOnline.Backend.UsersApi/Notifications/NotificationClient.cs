using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Support.Logging;
using NHSOnline.Backend.UsersApi.Notifications.Models;

namespace NHSOnline.Backend.UsersApi.Notifications
{
    internal class NotificationClient : INotificationClient
    {
        private readonly ILogger<NotificationClient> _logger;
        private readonly IAzureNotificationHubWrapperService _wrapperService;

        public NotificationClient(
            ILogger<NotificationClient> logger,
            IAzureNotificationHubWrapperService wrapperService
        )
        {
            _logger = logger;
            _wrapperService = wrapperService;
        }

        public async Task<string> CreateInstallation(InstallationRequest request)
        {
            _logger.LogEnter();

            try
            {
                var wrapper = _wrapperService.CurrentFor(request.NhsLoginId);

                var output = await wrapper.CreateInstallation(request);

                _logger.LogInformation($"Installation id {output} created for nhs login id {request.NhsLoginId} on hub {wrapper}");

                return output;
            }
            finally
            {
                _logger.LogExit();
            }
        }

        public async Task DeleteInstallation(string installationId, string nhsLoginId)
        {
            _logger.LogEnter();

            var tasks = _wrapperService.AllFor(nhsLoginId).Select(wrapper => Task.Run(async () =>
            {
                if (await wrapper.InstallationExists(installationId))
                {
                    await wrapper.DeleteInstallation(installationId);
                    _logger.LogInformation($"Installation Id {installationId} for nhs login id {nhsLoginId} deleted from hub {wrapper}");
                }
            }));

            await Task.WhenAll(tasks);

            _logger.LogExit();
        }

        public async Task DeleteInstallationsByDevicePns(string devicePns)
        {
            _logger.LogEnter();

            try
            {
                var tasks = _wrapperService.All().Select(wrapper => Task.Run(async () =>
                {
                    var installationIds = await wrapper.GetInstallationIdsByDevicePns(devicePns);

                    if (!installationIds.Any())
                    {
                        return;
                    }

                    await Task.WhenAll(installationIds.Select(async x => await wrapper.DeleteInstallation(x)));
                    _logger.LogInformation($"Installation ids {string.Join(", ", installationIds)} for device {devicePns} deleted from hub {wrapper}");
                }));

                await Task.WhenAll(tasks);
            }
            finally
            {
                _logger.LogExit();
            }
        }

        public async Task<bool> InstallationExists(string installationId, string nhsLoginId)
        {
            foreach (var wrapper in _wrapperService.AllFor(nhsLoginId))
            {
                if (await wrapper.InstallationExists(installationId))
                {
                    return true;
                }
            }

            return false;
        }

        public async Task<ICollection<string>> FindInstallationIdsByNhsLoginId(string nhsLoginId)
        {
            var output = new List<string>();

            foreach (var wrapper in _wrapperService.AllFor(nhsLoginId))
            {
                output.AddRange(await wrapper.GetInstallationIdsByNhsLoginId(nhsLoginId));
            }

            return output;
        }

        public async Task SendNotification(NotificationRequest request)
        {
            _logger.LogEnter();

            try
            {
                var wrappers = _wrapperService.AllFor(request.NhsLoginId).ToList();

                if (wrappers.Count == 1)
                {
                    await wrappers[0].SendNotification(request);
                    _logger.LogInformation($"Notification {request} sent to nhs login id {request.NhsLoginId} on hub {wrappers[0]}");
                    return;
                }

                foreach (var wrapper in wrappers)
                {
                    var installationIds = await wrapper.GetInstallationIdsByNhsLoginId(request.NhsLoginId);

                    if (!installationIds.Any())
                    {
                        continue;
                    }

                    await wrapper.SendNotification(request);
                    _logger.LogInformation($"Notification {request} sent to nhs login id {request.NhsLoginId} on hub {wrapper}");
                    return;
                }
            }
            finally
            {
                _logger.LogExit();
            }
        }
    }
}