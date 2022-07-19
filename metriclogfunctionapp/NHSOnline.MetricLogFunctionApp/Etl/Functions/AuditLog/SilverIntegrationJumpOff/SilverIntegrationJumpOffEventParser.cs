using System;
using System.Linq;

namespace NHSOnline.MetricLogFunctionApp.Etl.Functions.AuditLog.SilverIntegrationJumpOff;

public class SilverIntegrationJumpOffEventParser : IAuditLogParser<SilverIntegrationJumpOffMetric>
{
        private const string OperationFieldValue = "SilverIntegration_JumpOff_Click";
        private const string DetailsFieldValue = "The user has jumped off to an integration partner";

        public SilverIntegrationJumpOffMetric Parse(AuditRecord source)
        {
            if (!IsSilverIntegrationJumpOffMetric(source)) return null;

            return new SilverIntegrationJumpOffMetric
            {
                Timestamp = source.Timestamp,
                SessionId = source.SessionId,
                ProviderId = source.ProviderId,
                ProviderName = source.ProviderName,
                JumpOffId = source.JumpOffId,
                AuditId = source.AuditId
            };
        }

        private bool IsSilverIntegrationJumpOffMetric(AuditRecord source)
        {
            if (source == null) return false;

            return source.Operation == OperationFieldValue &&
                   source?.Details == DetailsFieldValue;
        }
}
