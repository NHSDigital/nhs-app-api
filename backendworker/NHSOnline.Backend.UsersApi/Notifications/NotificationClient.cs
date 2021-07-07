using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Support.Logging;
using NHSOnline.Backend.UsersApi.Areas.Devices.Models;
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

        public async Task<string> CreateInstallation(InstallationRequest request, string targetPath = null)
        {
            _logger.LogEnter();

            try
            {
                var wrapper = targetPath == null
                    ? _wrapperService.CurrentFor(request.NhsLoginId)
                    : _wrapperService.Hub(targetPath);

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

        public async Task<(HttpStatusCode, string)> Migrate(MigrationRequest request)
        {
            var message = string.Empty;

            try
            {
                var sourceWrapper = _wrapperService.Hub(request.SourcePath);
                var targetWrapper = _wrapperService.Hub(request.TargetPath);

                if (sourceWrapper.CanWriteFor(request.NhsLoginId))
                {
                    message = $"Installation Id {request.InstallationId} for nhs login id {request.NhsLoginId} correctly installed on {sourceWrapper}, no action taken";
                    return (HttpStatusCode.BadRequest, message);
                }

                if (!targetWrapper.CanWriteFor(request.NhsLoginId))
                {
                    message = $"Installation Id {request.InstallationId} for nhs login id {request.NhsLoginId} cannot be installed on {targetWrapper}, no action taken";
                    return (HttpStatusCode.BadRequest, message);
                }

                if (!await sourceWrapper.InstallationExists(request.InstallationId))
                {
                    message = $"Installation Id {request.InstallationId} for nhs login id {request.NhsLoginId} not found on {sourceWrapper}, no action taken";
                    return (HttpStatusCode.BadRequest, message);
                }

                var newInstallationId = await CreateInstallation(new InstallationRequest(request), request.TargetPath);

                if (string.IsNullOrEmpty(newInstallationId))
                {
                    message = $"Failed to create new registration for installation Id {request.InstallationId}, nhs login id {request.NhsLoginId}, leaving existing installation in place";
                    return (HttpStatusCode.BadGateway, message);
                }

                await sourceWrapper.DeleteInstallation(request.InstallationId);

                message = $"Installation Id {request.InstallationId} for nhs login id {request.NhsLoginId} deleted from hub {sourceWrapper}";
                return (HttpStatusCode.OK, newInstallationId);
            }
            finally
            {
                _logger.LogInformation(message);
            }
        }

        public async Task SendNotification(NotificationRequest request)
        {
            _logger.LogEnter();

            try
            {
                var wrappers = _wrapperService.AllFor(request.NhsLoginId).ToList();

                if (wrappers.Count == 1)
                {
                    await SendNotification(wrappers[0], request);

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

                    await SendNotification(wrapper, request);

                    _logger.LogInformation($"Notification {request} sent to nhs login id {request.NhsLoginId} on hub {wrapper}");
                    return;
                }
            }
            finally
            {
                _logger.LogExit();
            }
        }

        private static async Task SendNotification(IAzureNotificationHubWrapper wrapper, NotificationRequest request)
        {
            if (request.ScheduledTime != null)
            {
                await wrapper.SendScheduledNotification(request);
            }
            else
            {
                await wrapper.SendNotification(request);
            }
        }
    }
}