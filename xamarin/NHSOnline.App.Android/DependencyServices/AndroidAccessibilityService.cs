using Android.Content;
using Android.Views.Accessibility;
using Java.Lang;
using NHSOnline.App.DependencyServices;
using NHSOnline.App.Droid.DependencyServices;
using Xamarin.Forms;

[assembly: Dependency(typeof(AndroidAccessibilityService))]
namespace NHSOnline.App.Droid.DependencyServices
{
    public class AndroidAccessibilityService: IAccessibilityService
    {
        internal static MainActivity? MainActivity { get; set; }
        public void AnnounceText(string text)
        {
            var accessibilityManager = (AccessibilityManager?)MainActivity?.GetSystemService(Context.AccessibilityService);
            if (accessibilityManager?.IsEnabled is true)
            {
                var announcementEvent = AccessibilityEvent.Obtain();
                if (announcementEvent != null)
                {
                    announcementEvent.EventType = EventTypes.Announcement;
                    using var announcedText = new String(text);
                    announcementEvent.Text?.Add(announcedText);
                    accessibilityManager.SendAccessibilityEvent(announcementEvent);
                }
            }
        }
    }
}