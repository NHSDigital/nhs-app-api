using System;
using System.Diagnostics.CodeAnalysis;

namespace NHSOnline.Backend.Metrics.EventHub
{
    [Flags]
    [SuppressMessage("Microsoft.Naming", "CA1724", Justification = "Intentionally retaining name")]
    public enum EventHubs
    {
        None = 0,
        CommsHubNonPid = 1 << 0,
        CommsHubPid = 1 << 1,
        CommsHubBoth = CommsHubNonPid | CommsHubPid
    }
}