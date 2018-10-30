using System;
using System.Threading;

namespace NHSOnline.Backend.Worker.UnitTests.GpSystems.Suppliers.Vision
{
    public static class VisionDateTimeExtensions
    {
        public static string ToVisionDateTimeString(this DateTimeOffset dateTime)
        {
            return dateTime.ToString("yyyy-MM-ddTHH:mm:00", Thread.CurrentThread.CurrentCulture);
        }
    }
}