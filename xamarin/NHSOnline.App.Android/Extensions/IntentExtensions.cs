using Android.Content;

namespace NHSOnline.App.Droid.Extensions
{
    public static class IntentExtensions
    {
        private const string UrlDataKey = "url";

        public static string? GetDeepLink(this Intent? intent)
        {
            return intent?.Extras?.GetString(UrlDataKey);
        }

        public static void AddDeepLink(this Intent intent, string deepLink)
        {
            intent.PutExtra(UrlDataKey, deepLink);
        }
    }
}