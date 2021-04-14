using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Android.App;
using Android.Content;
using Android.OS;
using AndroidX.Core.App;
using AndroidX.Core.Content;
using Firebase.Messaging;
using Microsoft.Extensions.Logging;
using NHSOnline.App.Logging;

namespace NHSOnline.App.Droid.AndroidServices
{
    [Service]
    [IntentFilter(new[] { "com.google.firebase.MESSAGING_EVENT" })]
    public class AndroidNotificationMessagingService : FirebaseMessagingService
    {
        private static ILogger Logger => NhsAppLogging.CreateLogger(typeof(AndroidNotificationMessagingService));

        private const string ChannelId = "101";
        private const string UrlDataKey = "url";

        [SuppressMessage("Design", "CA1725: Parameter names should match base declaration",
            Justification = "p0 is not a descriptive name for the parameter")]
        public override void OnMessageReceived(RemoteMessage remoteMessage)
        {
            var notification = remoteMessage.GetNotification();
            if (notification is null)
            {
                Logger.LogInformation("Message does not contain a notification");
                return;
            }

            ProcessNotification(notification, remoteMessage.Data);
        }

        private void ProcessNotification(RemoteMessage.Notification notification,
            IDictionary<string, string> notificationData)
        {
            var notificationId = Guid.NewGuid().GetHashCode();
            var pendingIntent = BuildIntent(notificationId, notificationData);
            if (pendingIntent is null)
            {
                Logger.LogWarning("Cannot create intent for notification");
                return;
            }

            var builder = BuildNotification(pendingIntent, notification);
            CreateNotificationChannel();
            SendNotification(notificationId, builder);
        }

        private void CreateNotificationChannel()
        {
            if (Build.VERSION.SdkInt < BuildVersionCodes.O)
            {
                return;
            }

            using var channel = BuildChannel();
            var notificationManager = GetSystemService(NotificationService) as NotificationManager;
            notificationManager?.CreateNotificationChannel(channel);
        }

        private PendingIntent? BuildIntent(int notificationId, IDictionary<string, string> data)
        {
            using var intent = new Intent(this, typeof(MainActivity));
            intent.AddFlags(ActivityFlags.NewTask);

            if (data.TryGetValue(UrlDataKey, out var url) && !string.IsNullOrWhiteSpace(url))
            {
                intent.PutExtra(UrlDataKey, url);
            }

            return PendingIntent.GetActivity(this, notificationId, intent, PendingIntentFlags.OneShot);
        }

        private Notification BuildNotification(PendingIntent intent, RemoteMessage.Notification notification)
        {
            using var notificationBuilder = new NotificationCompat.Builder(this, ChannelId);
            notificationBuilder
                .SetSmallIcon(Resource.Drawable.icon_notifications_nhs_logo)
                .SetColorized(true)
                .SetColor(ContextCompat.GetColor(this, Resource.Color.colornotificationsprimary))
                .SetContentTitle(notification.Title)
                .SetContentText(notification.Body)
                .SetPriority(NotificationCompat.PriorityDefault)
                .SetAutoCancel(true)
                .SetContentIntent(intent);

            return notificationBuilder.Build();
        }

        private static NotificationChannel BuildChannel() =>
            new NotificationChannel(ChannelId,
                "NHS Notifications",
                NotificationImportance.Default)
            {
                Description = "Notifications from the NHS."
            };

        private void SendNotification(int notificationId, Notification notification)
        {
            var notificationManager = NotificationManagerCompat.From(this);
            notificationManager.Notify(notificationId, notification);
        }
    }
}