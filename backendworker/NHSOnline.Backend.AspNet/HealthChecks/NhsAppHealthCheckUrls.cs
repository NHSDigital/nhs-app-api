namespace NHSOnline.Backend.AspNet.HealthChecks
{
    internal static class NhsAppHealthCheckUrls
    {
        internal const string LivenessPath = "/health/live";
        internal const string ReadinessPath = "/health/ready";
    }
}
