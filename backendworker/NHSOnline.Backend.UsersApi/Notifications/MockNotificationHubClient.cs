using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.NotificationHubs;

namespace NHSOnline.Backend.UsersApi.Notifications
{
#pragma warning disable CA1054
    [SuppressMessage("ReSharper", "ExpressionIsAlwaysNull")]
    public class MockNotificationHubClient : INotificationHubClient
    {
        private readonly int _sleepTimeInMilliSeconds;

        private void SimulateDelay()
        {
            if (_sleepTimeInMilliSeconds > 0)
            {
                Thread.Sleep(_sleepTimeInMilliSeconds);
            }
        }

        public MockNotificationHubClient(int sleepTimeInMilliSeconds)
        {
            _sleepTimeInMilliSeconds = sleepTimeInMilliSeconds;
        }

        public Task CancelNotificationAsync(string scheduledNotificationId)
        {
            SimulateDelay();
            return Task.CompletedTask;
        }

        public Task CancelNotificationAsync(string scheduledNotificationId, CancellationToken cancellationToken)
        {
            SimulateDelay();
            return Task.CompletedTask;
        }

        public Task<AdmRegistrationDescription> CreateAdmNativeRegistrationAsync(string admRegistrationId)
        {
            SimulateDelay();
            return Task.FromResult(new AdmRegistrationDescription(Guid.NewGuid().ToString()));
        }

        public Task<AdmRegistrationDescription> CreateAdmNativeRegistrationAsync(string admRegistrationId,
            CancellationToken cancellationToken)
        {
            SimulateDelay();
            return Task.FromResult(new AdmRegistrationDescription(Guid.NewGuid().ToString()));
        }

        public Task<AdmRegistrationDescription> CreateAdmNativeRegistrationAsync(string admRegistrationId,
            IEnumerable<string> tags)
        {
            SimulateDelay();
            return Task.FromResult(new AdmRegistrationDescription(Guid.NewGuid().ToString(), tags));
        }

        public Task<AdmRegistrationDescription> CreateAdmNativeRegistrationAsync(string admRegistrationId,
            IEnumerable<string> tags, CancellationToken cancellationToken)
        {
            return CreateAdmNativeRegistrationAsync(admRegistrationId, tags);
        }

        public Task<AdmTemplateRegistrationDescription> CreateAdmTemplateRegistrationAsync(string admRegistrationId,
            string jsonPayload)
        {
            SimulateDelay();
            return Task.FromResult(new AdmTemplateRegistrationDescription(admRegistrationId));
        }

        public Task<AdmTemplateRegistrationDescription> CreateAdmTemplateRegistrationAsync(string admRegistrationId,
            string jsonPayload, CancellationToken cancellationToken)
        {
            SimulateDelay();
            return Task.FromResult(new AdmTemplateRegistrationDescription(admRegistrationId, jsonPayload));
        }

        public Task<AdmTemplateRegistrationDescription> CreateAdmTemplateRegistrationAsync(string admRegistrationId,
            string jsonPayload, IEnumerable<string> tags)
        {
            SimulateDelay();
            return Task.FromResult(new AdmTemplateRegistrationDescription(admRegistrationId, jsonPayload, tags));
        }

        public Task<AdmTemplateRegistrationDescription> CreateAdmTemplateRegistrationAsync(string admRegistrationId,
            string jsonPayload, IEnumerable<string> tags, CancellationToken cancellationToken)
        {
            return CreateAdmTemplateRegistrationAsync(admRegistrationId, jsonPayload, tags);
        }

        public Task<AppleRegistrationDescription> CreateAppleNativeRegistrationAsync(string deviceToken)
        {
            SimulateDelay();
            return Task.FromResult(new AppleRegistrationDescription(deviceToken));
        }

        public Task<AppleRegistrationDescription> CreateAppleNativeRegistrationAsync(string deviceToken,
            CancellationToken cancellationToken)
        {
            SimulateDelay();
            return Task.FromResult(new AppleRegistrationDescription(deviceToken));
        }

        public Task<AppleRegistrationDescription> CreateAppleNativeRegistrationAsync(string deviceToken,
            IEnumerable<string> tags)
        {
            SimulateDelay();
            return Task.FromResult(new AppleRegistrationDescription(deviceToken, tags));
        }

        public Task<AppleRegistrationDescription> CreateAppleNativeRegistrationAsync(string deviceToken,
            IEnumerable<string> tags, CancellationToken cancellationToken)
        {
            return CreateAppleNativeRegistrationAsync(deviceToken, tags);
        }

        public Task<AppleTemplateRegistrationDescription> CreateAppleTemplateRegistrationAsync(string deviceToken,
            string jsonPayload)
        {
            SimulateDelay();
            return Task.FromResult(new AppleTemplateRegistrationDescription(deviceToken, jsonPayload));
        }

        public Task<AppleTemplateRegistrationDescription> CreateAppleTemplateRegistrationAsync(string deviceToken,
            string jsonPayload, CancellationToken cancellationToken)
        {
            return CreateAppleTemplateRegistrationAsync(deviceToken, jsonPayload);
        }

        public Task<AppleTemplateRegistrationDescription> CreateAppleTemplateRegistrationAsync(string deviceToken,
            string jsonPayload, IEnumerable<string> tags)
        {
            SimulateDelay();
            return Task.FromResult(new AppleTemplateRegistrationDescription(deviceToken, jsonPayload, tags));
        }

        public Task<AppleTemplateRegistrationDescription> CreateAppleTemplateRegistrationAsync(string deviceToken,
            string jsonPayload, IEnumerable<string> tags, CancellationToken cancellationToken)
        {
            SimulateDelay();
            return Task.FromResult(new AppleTemplateRegistrationDescription(deviceToken, jsonPayload, tags));
        }

        public Task<BaiduRegistrationDescription> CreateBaiduNativeRegistrationAsync(string userId, string channelId)
        {
            SimulateDelay();
            return CreateBaiduNativeRegistrationAsync(userId, channelId, Enumerable.Empty<string>());
        }

        public Task<BaiduRegistrationDescription> CreateBaiduNativeRegistrationAsync(string userId, string channelId,
            IEnumerable<string> tags)
        {
            return Task.FromResult(new BaiduRegistrationDescription(userId, channelId, tags));
        }

        public Task<BaiduTemplateRegistrationDescription> CreateBaiduTemplateRegistrationAsync(string userId,
            string channelId, string jsonPayload)
        {
            SimulateDelay();
            return Task.FromResult(new BaiduTemplateRegistrationDescription(userId, channelId, jsonPayload));
        }

        public Task<BaiduTemplateRegistrationDescription> CreateBaiduTemplateRegistrationAsync(string userId,
            string channelId, string jsonPayload, IEnumerable<string> tags)
        {
            SimulateDelay();
            return Task.FromResult(new BaiduTemplateRegistrationDescription(userId, channelId, jsonPayload, tags));
        }

        public Task<FcmRegistrationDescription> CreateFcmNativeRegistrationAsync(string fcmRegistrationId)
        {
            SimulateDelay();
            return Task.FromResult(new FcmRegistrationDescription(fcmRegistrationId));
        }

        public Task<FcmRegistrationDescription> CreateFcmNativeRegistrationAsync(string fcmRegistrationId,
            CancellationToken cancellationToken)
        {
            return CreateFcmNativeRegistrationAsync(fcmRegistrationId);
        }

        public Task<FcmRegistrationDescription> CreateFcmNativeRegistrationAsync(string fcmRegistrationId,
            IEnumerable<string> tags)
        {
            SimulateDelay();
            return Task.FromResult(new FcmRegistrationDescription(fcmRegistrationId, tags));
        }

        public Task<FcmRegistrationDescription> CreateFcmNativeRegistrationAsync(string fcmRegistrationId,
            IEnumerable<string> tags, CancellationToken cancellationToken)
        {
            return CreateFcmNativeRegistrationAsync(fcmRegistrationId, tags);
        }

        public Task<FcmTemplateRegistrationDescription> CreateFcmTemplateRegistrationAsync(string fcmRegistrationId,
            string jsonPayload)
        {
            SimulateDelay();
            return Task.FromResult(new FcmTemplateRegistrationDescription(fcmRegistrationId, jsonPayload));
        }

        public Task<FcmTemplateRegistrationDescription> CreateFcmTemplateRegistrationAsync(string fcmRegistrationId,
            string jsonPayload, CancellationToken cancellationToken)
        {
            return CreateFcmTemplateRegistrationAsync(fcmRegistrationId, jsonPayload);
        }

        public Task<FcmTemplateRegistrationDescription> CreateFcmTemplateRegistrationAsync(string fcmRegistrationId,
            string jsonPayload, IEnumerable<string> tags)
        {
            SimulateDelay();
            return Task.FromResult(new FcmTemplateRegistrationDescription(fcmRegistrationId, jsonPayload, tags));
        }

        public Task<FcmTemplateRegistrationDescription> CreateFcmTemplateRegistrationAsync(string fcmRegistrationId,
            string jsonPayload, IEnumerable<string> tags, CancellationToken cancellationToken)
        {
            return CreateFcmTemplateRegistrationAsync(fcmRegistrationId, jsonPayload, tags);
        }

        public Task<MpnsRegistrationDescription> CreateMpnsNativeRegistrationAsync(string channelUri)
        {
            SimulateDelay();
            return Task.FromResult(new MpnsRegistrationDescription(channelUri));
        }

        public Task<MpnsRegistrationDescription> CreateMpnsNativeRegistrationAsync(string channelUri,
            IEnumerable<string> tags)
        {
            SimulateDelay();
            return Task.FromResult(new MpnsRegistrationDescription(channelUri, tags));
        }

        public Task<MpnsTemplateRegistrationDescription> CreateMpnsTemplateRegistrationAsync(string channelUri,
            string xmlTemplate)
        {
            SimulateDelay();
            return Task.FromResult(new MpnsTemplateRegistrationDescription(channelUri, xmlTemplate));
        }

        public Task<MpnsTemplateRegistrationDescription> CreateMpnsTemplateRegistrationAsync(string channelUri,
            string xmlTemplate, IEnumerable<string> tags)
        {
            SimulateDelay();
            return Task.FromResult(new MpnsTemplateRegistrationDescription(channelUri, xmlTemplate, tags));
        }

        public void CreateOrUpdateInstallation(Installation installation)
        {
            SimulateDelay();
        }

        public Task CreateOrUpdateInstallationAsync(Installation installation)
        {
            SimulateDelay();
            return Task.CompletedTask;
        }

        public Task CreateOrUpdateInstallationAsync(Installation installation, CancellationToken cancellationToken)
        {
            SimulateDelay();
            return Task.CompletedTask;
        }

        public Task<T> CreateOrUpdateRegistrationAsync<T>(T registration) where T : RegistrationDescription
        {
            SimulateDelay();
            return Task.FromResult(registration);
        }

        public Task<T> CreateOrUpdateRegistrationAsync<T>(T registration, CancellationToken cancellationToken)
            where T : RegistrationDescription
        {
            SimulateDelay();
            return Task.FromResult(registration);
        }

        public Task<T> CreateRegistrationAsync<T>(T registration) where T : RegistrationDescription
        {
            SimulateDelay();
            return Task.FromResult(registration);
        }

        public Task<T> CreateRegistrationAsync<T>(T registration, CancellationToken cancellationToken)
            where T : RegistrationDescription
        {
            SimulateDelay();
            return Task.FromResult(registration);
        }

        public Task<string> CreateRegistrationIdAsync()
        {
            SimulateDelay();
            return Task.FromResult(Guid.NewGuid().ToString());
        }

        public Task<string> CreateRegistrationIdAsync(CancellationToken cancellationToken)
        {
            SimulateDelay();
            return Task.FromResult(Guid.NewGuid().ToString());
        }

        public Task<WindowsRegistrationDescription> CreateWindowsNativeRegistrationAsync(string channelUri)
        {
            SimulateDelay();
            return Task.FromResult(new WindowsRegistrationDescription(channelUri));
        }

        public Task<WindowsRegistrationDescription> CreateWindowsNativeRegistrationAsync(string channelUri,
            CancellationToken cancellationToken)
        {
            return CreateWindowsNativeRegistrationAsync(channelUri);
        }

        public Task<WindowsRegistrationDescription> CreateWindowsNativeRegistrationAsync(string channelUri,
            IEnumerable<string> tags)
        {
            SimulateDelay();
            return Task.FromResult(new WindowsRegistrationDescription(channelUri, tags));
        }

        public Task<WindowsRegistrationDescription> CreateWindowsNativeRegistrationAsync(string channelUri,
            IEnumerable<string> tags, CancellationToken cancellationToken)
        {
            return CreateWindowsNativeRegistrationAsync(channelUri, tags);
        }

        public Task<WindowsTemplateRegistrationDescription> CreateWindowsTemplateRegistrationAsync(string channelUri,
            string xmlTemplate)
        {
            SimulateDelay();
            return Task.FromResult(new WindowsTemplateRegistrationDescription(channelUri, xmlTemplate));
        }

        public Task<WindowsTemplateRegistrationDescription> CreateWindowsTemplateRegistrationAsync(string channelUri,
            string xmlTemplate, CancellationToken cancellationToken)
        {
            return CreateWindowsTemplateRegistrationAsync(channelUri, xmlTemplate);
        }

        public Task<WindowsTemplateRegistrationDescription> CreateWindowsTemplateRegistrationAsync(string channelUri,
            string xmlTemplate, IEnumerable<string> tags)
        {
            SimulateDelay();
            return Task.FromResult(new WindowsTemplateRegistrationDescription(channelUri, xmlTemplate, tags));
        }

        public Task<WindowsTemplateRegistrationDescription> CreateWindowsTemplateRegistrationAsync(string channelUri,
            string xmlTemplate, IEnumerable<string> tags, CancellationToken cancellationToken)
        {
            return CreateWindowsTemplateRegistrationAsync(channelUri, xmlTemplate, tags);
        }

        public void DeleteInstallation(string installationId)
        {
            SimulateDelay();
        }

        public Task DeleteInstallationAsync(string installationId)
        {
            SimulateDelay();
            return Task.CompletedTask;
        }

        public Task DeleteInstallationAsync(string installationId, CancellationToken cancellationToken)
        {
            SimulateDelay();
            return Task.CompletedTask;
        }

        public Task DeleteRegistrationAsync(RegistrationDescription registration)
        {
            SimulateDelay();
            return Task.CompletedTask;
        }

        public Task DeleteRegistrationAsync(RegistrationDescription registration, CancellationToken cancellationToken)
        {
            SimulateDelay();
            return Task.CompletedTask;
        }

        public Task DeleteRegistrationAsync(string registrationId)
        {
            SimulateDelay();
            return Task.CompletedTask;
        }

        public Task DeleteRegistrationAsync(string registrationId, CancellationToken cancellationToken)
        {
            SimulateDelay();
            return Task.CompletedTask;
        }

        public Task DeleteRegistrationAsync(string registrationId, string etag)
        {
            SimulateDelay();
            return Task.CompletedTask;
        }

        public Task DeleteRegistrationAsync(string registrationId, string etag, CancellationToken cancellationToken)
        {
            SimulateDelay();
            return Task.CompletedTask;
        }

        public Task DeleteRegistrationsByChannelAsync(string pnsHandle)
        {
            SimulateDelay();
            return Task.CompletedTask;
        }

        public Task DeleteRegistrationsByChannelAsync(string pnsHandle, CancellationToken cancellationToken)
        {
            SimulateDelay();
            return Task.CompletedTask;
        }

        public Task<CollectionQueryResult<RegistrationDescription>> GetAllRegistrationsAsync(int top)
        {
            SimulateDelay();
            return Task.FromResult(GetRegistrationDescriptionEnumerable(new List<RegistrationDescription>()));
        }

        public Task<CollectionQueryResult<RegistrationDescription>> GetAllRegistrationsAsync(int top,
            CancellationToken cancellationToken)
        {
            return GetAllRegistrationsAsync(top);
        }

        public Task<CollectionQueryResult<RegistrationDescription>> GetAllRegistrationsAsync(string continuationToken,
            int top)
        {
            return GetAllRegistrationsAsync(top);
        }

        public Task<CollectionQueryResult<RegistrationDescription>> GetAllRegistrationsAsync(string continuationToken,
            int top, CancellationToken cancellationToken)
        {
            return GetAllRegistrationsAsync(top, cancellationToken);
        }

        public Uri GetBaseUri()
        {
            SimulateDelay();
            return new Uri("sb://mock.notification.hub:9999");
        }

        public Task<Uri> GetFeedbackContainerUriAsync()
        {
            SimulateDelay();
            return Task.FromResult(new Uri("sb://mock.notification.hub:9999/feedback"));
        }

        public Task<Uri> GetFeedbackContainerUriAsync(CancellationToken cancellationToken)
        {
            return GetFeedbackContainerUriAsync();
        }

        public Installation GetInstallation(string installationId)
        {
            SimulateDelay();
            return new Installation { InstallationId = installationId };
        }

        public Task<Installation> GetInstallationAsync(string installationId)
        {
            SimulateDelay();
            return Task.FromResult(new Installation { InstallationId = installationId });
        }

        public Task<Installation> GetInstallationAsync(string installationId, CancellationToken cancellationToken)
        {
            return GetInstallationAsync(installationId);
        }

        public Task<NotificationHubJob> GetNotificationHubJobAsync(string jobId)
        {
            SimulateDelay();
            return Task.FromResult(new NotificationHubJob());
        }

        public Task<NotificationHubJob> GetNotificationHubJobAsync(string jobId, CancellationToken cancellationToken)
        {
            return GetNotificationHubJobAsync(jobId);
        }

        public async Task<IEnumerable<NotificationHubJob>> GetNotificationHubJobsAsync()
        {
            SimulateDelay();
            return new List<NotificationHubJob>
            {
                await GetNotificationHubJobAsync(Guid.NewGuid().ToString()),
                await GetNotificationHubJobAsync(Guid.NewGuid().ToString())
            };
        }

        public Task<IEnumerable<NotificationHubJob>> GetNotificationHubJobsAsync(CancellationToken cancellationToken)
        {
            return GetNotificationHubJobsAsync();
        }

        public Task<NotificationDetails> GetNotificationOutcomeDetailsAsync(string notificationId)
        {
            SimulateDelay();
            return Task.FromResult(new NotificationDetails { NotificationId = notificationId });
        }

        public Task<NotificationDetails> GetNotificationOutcomeDetailsAsync(string notificationId,
            CancellationToken cancellationToken)
        {
            return GetNotificationOutcomeDetailsAsync(notificationId);
        }

        public Task<TRegistrationDescription> GetRegistrationAsync<TRegistrationDescription>(string registrationId)
            where TRegistrationDescription : RegistrationDescription
        {
            SimulateDelay();
            return Task.FromResult(new FcmRegistrationDescription(registrationId) as TRegistrationDescription);
        }

        public Task<TRegistrationDescription> GetRegistrationAsync<TRegistrationDescription>(string registrationId,
            CancellationToken cancellationToken) where TRegistrationDescription : RegistrationDescription
        {
            return GetRegistrationAsync<TRegistrationDescription>(registrationId);
        }

        public Task<CollectionQueryResult<RegistrationDescription>> GetRegistrationsByChannelAsync(string pnsHandle,
            int top)
        {
            SimulateDelay();
            return Task.FromResult(GetRegistrationDescriptionEnumerable(new List<RegistrationDescription>()));
        }

        public Task<CollectionQueryResult<RegistrationDescription>> GetRegistrationsByChannelAsync(string pnsHandle,
            int top, CancellationToken cancellationToken)
        {
            return GetRegistrationsByChannelAsync(pnsHandle, top);
        }

        public Task<CollectionQueryResult<RegistrationDescription>> GetRegistrationsByChannelAsync(string pnsHandle,
            string continuationToken, int top)
        {
            return GetRegistrationsByChannelAsync(pnsHandle, top);
        }

        public Task<CollectionQueryResult<RegistrationDescription>> GetRegistrationsByChannelAsync(string pnsHandle,
            string continuationToken, int top, CancellationToken cancellationToken)
        {
            return GetRegistrationsByChannelAsync(pnsHandle, continuationToken, top);
        }

        public Task<CollectionQueryResult<RegistrationDescription>> GetRegistrationsByTagAsync(string tag, int top)
        {
            var tags = new[]
            {
                $"$installationId:{{{Guid.NewGuid()}}}",
                tag,
                "odsCode:A12345"
            };
            var registration = new FcmRegistrationDescription("DevicePns", tags)
            {
                RegistrationId = Guid.NewGuid().ToString()
            };

            SimulateDelay();
            return Task.FromResult(GetRegistrationDescriptionEnumerable(new List<RegistrationDescription>
                { registration }));
        }

        public Task<CollectionQueryResult<RegistrationDescription>> GetRegistrationsByTagAsync(string tag, int top,
            CancellationToken cancellationToken)
        {
            return GetRegistrationsByTagAsync(tag, top);
        }

        public Task<CollectionQueryResult<RegistrationDescription>> GetRegistrationsByTagAsync(string tag,
            string continuationToken, int top)
        {
            return GetRegistrationsByTagAsync(tag, top);
        }

        public Task<CollectionQueryResult<RegistrationDescription>> GetRegistrationsByTagAsync(string tag,
            string continuationToken, int top, CancellationToken cancellationToken)
        {
            return GetRegistrationsByTagAsync(tag, top, cancellationToken);
        }

        public bool InstallationExists(string installationId)
        {
            SimulateDelay();
            return true;
        }

        public Task<bool> InstallationExistsAsync(string installationId)
        {
            SimulateDelay();
            return Task.FromResult(true);
        }

        public Task<bool> InstallationExistsAsync(string installationId, CancellationToken cancellationToken)
        {
            SimulateDelay();
            return InstallationExistsAsync(installationId);
        }

        public void PatchInstallation(string installationId, IList<PartialUpdateOperation> operations)
        {
            SimulateDelay();
        }

        public Task PatchInstallationAsync(string installationId, IList<PartialUpdateOperation> operations)
        {
            SimulateDelay();
            return Task.CompletedTask;
        }

        public Task PatchInstallationAsync(string installationId, IList<PartialUpdateOperation> operations,
            CancellationToken cancellationToken)
        {
            SimulateDelay();
            return Task.CompletedTask;
        }

        public Task<bool> RegistrationExistsAsync(string registrationId)
        {
            SimulateDelay();
            return Task.FromResult(true);
        }

        public Task<bool> RegistrationExistsAsync(string registrationId, CancellationToken cancellationToken)
        {
            return RegistrationExistsAsync(registrationId);
        }

        public Task<ScheduledNotification> ScheduleNotificationAsync(Notification notification,
            DateTimeOffset scheduledTime)
        {
            SimulateDelay();
            return Task.FromResult(new ScheduledNotification());
        }

        public Task<ScheduledNotification> ScheduleNotificationAsync(Notification notification,
            DateTimeOffset scheduledTime, CancellationToken cancellationToken)
        {
            return ScheduleNotificationAsync(notification, scheduledTime);
        }

        public Task<ScheduledNotification> ScheduleNotificationAsync(Notification notification,
            DateTimeOffset scheduledTime, IEnumerable<string> tags)
        {
            return ScheduleNotificationAsync(notification, scheduledTime);
        }

        public Task<ScheduledNotification> ScheduleNotificationAsync(Notification notification,
            DateTimeOffset scheduledTime, IEnumerable<string> tags, CancellationToken cancellationToken)
        {
            return ScheduleNotificationAsync(notification, scheduledTime, tags);
        }

        public Task<ScheduledNotification> ScheduleNotificationAsync(Notification notification,
            DateTimeOffset scheduledTime, string tagExpression)
        {
            return ScheduleNotificationAsync(notification, scheduledTime);
        }

        public Task<ScheduledNotification> ScheduleNotificationAsync(Notification notification,
            DateTimeOffset scheduledTime, string tagExpression, CancellationToken cancellationToken)
        {
            return ScheduleNotificationAsync(notification, scheduledTime, tagExpression);
        }

        public Task<NotificationOutcome> SendAdmNativeNotificationAsync(string jsonPayload)
        {
            SimulateDelay();
            return Task.FromResult(GetDefaultNotificationOutCome());
        }

        public Task<NotificationOutcome> SendAdmNativeNotificationAsync(string jsonPayload,
            CancellationToken cancellationToken)
        {
            return SendAdmNativeNotificationAsync(jsonPayload);
        }

        public Task<NotificationOutcome> SendAdmNativeNotificationAsync(string jsonPayload, IEnumerable<string> tags)
        {
            return SendAdmNativeNotificationAsync(jsonPayload);
        }

        public Task<NotificationOutcome> SendAdmNativeNotificationAsync(string jsonPayload, IEnumerable<string> tags,
            CancellationToken cancellationToken)
        {
            return SendAdmNativeNotificationAsync(jsonPayload, cancellationToken);
        }

        public Task<NotificationOutcome> SendAdmNativeNotificationAsync(string jsonPayload, string tagExpression)
        {
            return SendAdmNativeNotificationAsync(jsonPayload);
        }

        public Task<NotificationOutcome> SendAdmNativeNotificationAsync(string jsonPayload, string tagExpression,
            CancellationToken cancellationToken)
        {
            return SendAdmNativeNotificationAsync(jsonPayload, tagExpression);
        }

        public Task<NotificationOutcome> SendAppleNativeNotificationAsync(string jsonPayload)
        {
            SimulateDelay();
            return Task.FromResult(GetDefaultNotificationOutCome());
        }

        public Task<NotificationOutcome> SendAppleNativeNotificationAsync(string jsonPayload,
            CancellationToken cancellationToken)
        {
            return SendAppleNativeNotificationAsync(jsonPayload);
        }

        public Task<NotificationOutcome> SendAppleNativeNotificationAsync(string jsonPayload, IEnumerable<string> tags)
        {
            return SendAppleNativeNotificationAsync(jsonPayload);
        }

        public Task<NotificationOutcome> SendAppleNativeNotificationAsync(string jsonPayload, IEnumerable<string> tags,
            CancellationToken cancellationToken)
        {
            return SendAppleNativeNotificationAsync(jsonPayload, cancellationToken);
        }

        public Task<NotificationOutcome> SendAppleNativeNotificationAsync(string jsonPayload, string tagExpression)
        {
            return SendAppleNativeNotificationAsync(jsonPayload);
        }

        public Task<NotificationOutcome> SendAppleNativeNotificationAsync(string jsonPayload, string tagExpression,
            CancellationToken cancellationToken)
        {
            return SendAppleNativeNotificationAsync(jsonPayload, cancellationToken);
        }

        public Task<NotificationOutcome> SendBaiduNativeNotificationAsync(string message)
        {
            SimulateDelay();
            return Task.FromResult(GetDefaultNotificationOutCome());
        }

        public Task<NotificationOutcome> SendBaiduNativeNotificationAsync(string message,
            CancellationToken cancellationToken)
        {
            return SendBaiduNativeNotificationAsync(message);
        }

        public Task<NotificationOutcome> SendBaiduNativeNotificationAsync(string message, IEnumerable<string> tags)
        {
            return SendBaiduNativeNotificationAsync(message);
        }

        public Task<NotificationOutcome> SendBaiduNativeNotificationAsync(string message, IEnumerable<string> tags,
            CancellationToken cancellationToken)
        {
            return SendBaiduNativeNotificationAsync(message, cancellationToken);
        }

        public Task<NotificationOutcome> SendBaiduNativeNotificationAsync(string message, string tagExpression)
        {
            return SendBaiduNativeNotificationAsync(message);
        }

        public Task<NotificationOutcome> SendBaiduNativeNotificationAsync(string message, string tagExpression,
            CancellationToken cancellationToken)
        {
            return SendBaiduNativeNotificationAsync(message, cancellationToken);
        }

        public Task<NotificationOutcome> SendDirectNotificationAsync(Notification notification,
            IList<string> deviceHandles)
        {
            SimulateDelay();
            return Task.FromResult(GetDefaultNotificationOutCome());
        }

        public Task<NotificationOutcome> SendDirectNotificationAsync(Notification notification,
            IList<string> deviceHandles, CancellationToken cancellationToken)
        {
            return SendDirectNotificationAsync(notification, deviceHandles);
        }

        public Task<NotificationOutcome> SendDirectNotificationAsync(Notification notification, string deviceHandle)
        {
            return SendDirectNotificationAsync(notification, new List<string> { deviceHandle });
        }

        public Task<NotificationOutcome> SendDirectNotificationAsync(Notification notification, string deviceHandle,
            CancellationToken cancellationToken)
        {
            return SendDirectNotificationAsync(notification, deviceHandle);
        }

        public Task<NotificationOutcome> SendFcmNativeNotificationAsync(string jsonPayload)
        {
            SimulateDelay();
            return Task.FromResult(GetDefaultNotificationOutCome());
        }

        public Task<NotificationOutcome> SendFcmNativeNotificationAsync(string jsonPayload,
            CancellationToken cancellationToken)
        {
            return SendFcmNativeNotificationAsync(jsonPayload);
        }

        public Task<NotificationOutcome> SendFcmNativeNotificationAsync(string jsonPayload, IEnumerable<string> tags)
        {
            return SendFcmNativeNotificationAsync(jsonPayload);
        }

        public Task<NotificationOutcome> SendFcmNativeNotificationAsync(string jsonPayload, IEnumerable<string> tags,
            CancellationToken cancellationToken)
        {
            return SendFcmNativeNotificationAsync(jsonPayload, cancellationToken);
        }

        public Task<NotificationOutcome> SendFcmNativeNotificationAsync(string jsonPayload, string tagExpression)
        {
            return SendFcmNativeNotificationAsync(jsonPayload);
        }

        public Task<NotificationOutcome> SendFcmNativeNotificationAsync(string jsonPayload, string tagExpression,
            CancellationToken cancellationToken)
        {
            return SendFcmNativeNotificationAsync(jsonPayload, cancellationToken);
        }

        public Task<NotificationOutcome> SendMpnsNativeNotificationAsync(string nativePayload)
        {
            SimulateDelay();
            return Task.FromResult(GetDefaultNotificationOutCome());
        }

        public Task<NotificationOutcome> SendMpnsNativeNotificationAsync(string nativePayload,
            CancellationToken cancellationToken)
        {
            return SendMpnsNativeNotificationAsync(nativePayload);
        }

        public Task<NotificationOutcome> SendMpnsNativeNotificationAsync(string nativePayload, IEnumerable<string> tags)
        {
            return SendMpnsNativeNotificationAsync(nativePayload);
        }

        public Task<NotificationOutcome> SendMpnsNativeNotificationAsync(string nativePayload, IEnumerable<string> tags,
            CancellationToken cancellationToken)
        {
            return SendMpnsNativeNotificationAsync(nativePayload, cancellationToken);
        }

        public Task<NotificationOutcome> SendMpnsNativeNotificationAsync(string nativePayload, string tagExpression)
        {
            return SendMpnsNativeNotificationAsync(nativePayload);
        }

        public Task<NotificationOutcome> SendMpnsNativeNotificationAsync(string nativePayload, string tagExpression,
            CancellationToken cancellationToken)
        {
            return SendMpnsNativeNotificationAsync(nativePayload, cancellationToken);
        }

        public Task<NotificationOutcome> SendNotificationAsync(Notification notification)
        {
            SimulateDelay();
            return Task.FromResult(GetDefaultNotificationOutCome());
        }

        public Task<NotificationOutcome> SendNotificationAsync(Notification notification,
            CancellationToken cancellationToken)
        {
            return SendNotificationAsync(notification);
        }

        public Task<NotificationOutcome> SendNotificationAsync(Notification notification, IEnumerable<string> tags)
        {
            return SendNotificationAsync(notification);
        }

        public Task<NotificationOutcome> SendNotificationAsync(Notification notification, IEnumerable<string> tags,
            CancellationToken cancellationToken)
        {
            return SendNotificationAsync(notification, tags);
        }

        public Task<NotificationOutcome> SendNotificationAsync(Notification notification, string tagExpression)
        {
            return SendNotificationAsync(notification);
        }

        public Task<NotificationOutcome> SendNotificationAsync(Notification notification, string tagExpression,
            CancellationToken cancellationToken)
        {
            return SendNotificationAsync(notification, tagExpression);
        }

        public Task<NotificationOutcome> SendTemplateNotificationAsync(IDictionary<string, string> properties)
        {
            SimulateDelay();
            return Task.FromResult(GetDefaultNotificationOutCome());
        }

        public Task<NotificationOutcome> SendTemplateNotificationAsync(IDictionary<string, string> properties,
            CancellationToken cancellationToken)
        {
            return SendTemplateNotificationAsync(properties);
        }

        public Task<NotificationOutcome> SendTemplateNotificationAsync(IDictionary<string, string> properties,
            IEnumerable<string> tags)
        {
            SimulateDelay();
            return Task.FromResult(GetDefaultNotificationOutCome());
        }

        public Task<NotificationOutcome> SendTemplateNotificationAsync(IDictionary<string, string> properties,
            IEnumerable<string> tags, CancellationToken cancellationToken)
        {
            return SendTemplateNotificationAsync(properties, tags);
        }

        public Task<NotificationOutcome> SendTemplateNotificationAsync(IDictionary<string, string> properties,
            string tagExpression)
        {
            SimulateDelay();
            return Task.FromResult(GetDefaultNotificationOutCome());
        }

        public Task<NotificationOutcome> SendTemplateNotificationAsync(IDictionary<string, string> properties,
            string tagExpression, CancellationToken cancellationToken)
        {
            return SendTemplateNotificationAsync(properties, tagExpression);
        }

        public Task<NotificationOutcome> SendWindowsNativeNotificationAsync(string windowsNativePayload)
        {
            SimulateDelay();
            return Task.FromResult(GetDefaultNotificationOutCome());
        }

        public Task<NotificationOutcome> SendWindowsNativeNotificationAsync(string windowsNativePayload,
            CancellationToken cancellationToken)
        {
            return SendWindowsNativeNotificationAsync(windowsNativePayload);
        }

        public Task<NotificationOutcome> SendWindowsNativeNotificationAsync(string windowsNativePayload,
            IEnumerable<string> tags)
        {
            SimulateDelay();
            return Task.FromResult(GetDefaultNotificationOutCome());
        }

        public Task<NotificationOutcome> SendWindowsNativeNotificationAsync(string windowsNativePayload,
            IEnumerable<string> tags, CancellationToken cancellationToken)
        {
            return SendWindowsNativeNotificationAsync(windowsNativePayload, tags);
        }

        public Task<NotificationOutcome> SendWindowsNativeNotificationAsync(string windowsNativePayload,
            string tagExpression)
        {
            SimulateDelay();
            return Task.FromResult(GetDefaultNotificationOutCome());
        }

        public Task<NotificationOutcome> SendWindowsNativeNotificationAsync(string windowsNativePayload,
            string tagExpression, CancellationToken cancellationToken)
        {
            return SendWindowsNativeNotificationAsync(windowsNativePayload, tagExpression);
        }

        public Task<NotificationHubJob> SubmitNotificationHubJobAsync(NotificationHubJob job)
        {
            SimulateDelay();
            return Task.FromResult(job);
        }

        public Task<NotificationHubJob> SubmitNotificationHubJobAsync(NotificationHubJob job,
            CancellationToken cancellationToken)
        {
            SimulateDelay();
            return SubmitNotificationHubJobAsync(job);
        }

        public Task<T> UpdateRegistrationAsync<T>(T registration) where T : RegistrationDescription
        {
            SimulateDelay();
            return Task.FromResult(registration);
        }

        public Task<T> UpdateRegistrationAsync<T>(T registration, CancellationToken cancellationToken)
            where T : RegistrationDescription
        {
            SimulateDelay();
            return UpdateRegistrationAsync(registration);
        }

        public bool EnableTestSend => true;

        private static CollectionQueryResult<RegistrationDescription> GetRegistrationDescriptionEnumerable(
            List<RegistrationDescription> registrations)
        {
            /* Work around until  https://github.com/Azure/azure-notificationhubs-dotnet/pull/210
             is merged into Microsoft.Azure.NotificationHubs nuget package */

            const BindingFlags flags = BindingFlags.NonPublic | BindingFlags.Instance;
            var instance = Activator.CreateInstance(
                typeof(CollectionQueryResult<RegistrationDescription>),
                flags, null,
                new object[] { registrations, "" },
                CultureInfo.InvariantCulture);
            return instance as CollectionQueryResult<RegistrationDescription>;
        }

        private static NotificationOutcome GetDefaultNotificationOutCome()
        {
            return new NotificationOutcome { NotificationId = Guid.NewGuid().ToString() };
        }
    }
}