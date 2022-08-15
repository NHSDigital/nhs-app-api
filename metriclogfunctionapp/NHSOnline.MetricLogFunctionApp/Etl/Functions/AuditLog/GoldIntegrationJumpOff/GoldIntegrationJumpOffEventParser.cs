using System;
using System.Linq;

namespace NHSOnline.MetricLogFunctionApp.Etl.Functions.AuditLog.GoldIntegrationJumpOff;

public class GoldIntegrationJumpOffEventParser : IAuditLogParser<GoldIntegrationJumpOffMetric>
{
    private const string OperationFieldValue = "GoldIntegration_JumpOff_Click";
    private const string DetailsFieldValue = "The user has jumped off to an integration partner";

    public GoldIntegrationJumpOffMetric Parse(AuditRecord source)
    {
        if (!IsGoldIntegrationJumpOffMetric(source)) return null;

        return new GoldIntegrationJumpOffMetric
        {
            Timestamp = source.Timestamp,
            SessionId = source.SessionId,
            ProviderId = source.ProviderId,
            ProviderName = source.ProviderName,
            JumpOffId = source.JumpOffId,
            AuditId = source.AuditId
        };
    }

    private bool IsGoldIntegrationJumpOffMetric(AuditRecord source)
    {
        if (source == null) return false;

        return source.Operation == OperationFieldValue &&
               source.Details == DetailsFieldValue;
    }
}
