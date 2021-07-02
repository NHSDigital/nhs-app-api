using System;
using System.Diagnostics.CodeAnalysis;

namespace NHSOnline.IntegrationTestAnalyser.Models
{
    public class CustomTestReport
    {
        public string? BrowserStackSessionId { get; set; }

        [SuppressMessage("ReSharper", "CA1056",  Justification = "URL string required as it is for display purposes only")]
        public string? BrowserStackSessionUrl { get; set; }

        public string? MethodName { get; set; }

        public string? Description { get; set; }

        public string? TestFailureMessage { get; set; }

        public string? Outcome { get; set; }

        public bool IsManual => Outcome!.Equals("Inconclusive", StringComparison.Ordinal);
    }
}