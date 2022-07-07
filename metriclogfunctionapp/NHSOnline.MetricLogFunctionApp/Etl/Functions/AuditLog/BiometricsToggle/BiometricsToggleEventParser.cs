using System.Text.RegularExpressions;

namespace NHSOnline.MetricLogFunctionApp.Etl.Functions.AuditLog.BiometricsToggle;

public class BiometricsToggleEventParser: IAuditLogParser<BiometricsToggleMetric>
{
    private const string OperationFieldValue = "BiometricsRegistration_Decision";
    private const string DetailsFieldValue = "Biometrics toggled.";

    public BiometricsToggleMetric Parse(AuditRecord source)
    {
        if (!IsBiometricsToggleMetric(source)) return null;

        return new BiometricsToggleMetric
        {
            SessionId = source.SessionId,
            Timestamp = source.Timestamp,
            BiometricsToggle = GetOptedInValueFromAuditRecordDetails(source.Details),
            AuditId = source.AuditId
        };
    }

    private string GetOptedInValueFromAuditRecordDetails(string details)
    {
        Regex optedInPattern = new Regex(@"optIn=(?<optedIn>.*)");
        Match optedInMatch = optedInPattern.Match(details);

        return optedInMatch.Groups["optedIn"].Value switch
        {
            "True" => "On",
            _ => "Off"
        };
    }

    private bool IsBiometricsToggleMetric(AuditRecord source)
    {
        if (source == null) return false;

        return source.Operation == OperationFieldValue &&
               source.Details != null && source.Details.Contains(DetailsFieldValue);
    }
}
