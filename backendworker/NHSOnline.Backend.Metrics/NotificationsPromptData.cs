using System.Collections.Generic;
using System.Globalization;

namespace NHSOnline.Backend.Metrics
{
    public class NotificationsPromptData : IMetricData
    {
        public bool ScreenShown { get; }
        public bool NotificationsRegistered { get; }
        public string Platform { get; }

        public NotificationsPromptData(bool screenShown, bool notificationsRegistered, string platform)
        {
            ScreenShown = screenShown;
            NotificationsRegistered = notificationsRegistered;
            Platform = platform;
        }

        public IEnumerable<KeyValuePair<string, string>> ToKeyValuePairs()
        {
            yield return new KeyValuePair<string, string>("ScreenShown", ScreenShown.ToString(CultureInfo.InvariantCulture));
            yield return new KeyValuePair<string, string>("NotificationsRegistered", NotificationsRegistered.ToString(CultureInfo.InvariantCulture));
            yield return new KeyValuePair<string, string>("Platform", Platform);
        }
    }
}