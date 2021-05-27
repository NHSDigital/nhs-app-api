using System.Collections.Generic;
using System.Linq;

namespace NHSOnline.Backend.AspNet.HealthChecks
{
    public static class NhsAppHealthCheckTags
    {
        public const string LivenessValue = "liveness";
        public const string ReadinessValue = "readiness";

        public static readonly IEnumerable<string> Liveness = new List<string> { LivenessValue };
        public static readonly IEnumerable<string> Readiness = new List<string> { ReadinessValue };
        public static readonly IEnumerable<string> LivenessAndReadiness = Liveness.Concat(Readiness).ToList();
    }
}
