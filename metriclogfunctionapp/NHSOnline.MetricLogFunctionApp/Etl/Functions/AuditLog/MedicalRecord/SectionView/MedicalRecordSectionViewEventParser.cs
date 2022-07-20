using System;
using System.Linq;

namespace NHSOnline.MetricLogFunctionApp.Etl.Functions.AuditLog.MedicalRecord.SectionView;

public class MedicalRecordSectionViewEventParser : IAuditLogParser<MedicalRecordSectionViewMetric>
{
    private const string OperationFieldValue = "PatientRecord_Section_View_Response";
    private const string DetailsFieldBeginning = "Patient record ";
    private const string DetailsFieldEnding = " successfully retrieved.";

    public MedicalRecordSectionViewMetric Parse(AuditRecord source)
    {
        if (!IsMedicalRecordSectionViewMetric(source) ||
            !IsSuccessfullyRetrieved(source))
        {
            return null;
        }

        var section = GetSectionValueFromAuditRecordDetails(source.Details);
        if (section == null) return null;

        return new MedicalRecordSectionViewMetric
        {
            Timestamp = source.Timestamp,
            SessionId = source.SessionId,
            Supplier = source.Supplier,
            IsActingOnBehalfOfAnother = source.IsActingOnBehalfOfAnother,
            Section = section,
            AuditId = source.AuditId
        };
    }

    private bool IsMedicalRecordSectionViewMetric(AuditRecord source)
    {
        if (source == null) return false;

        return source.Operation == OperationFieldValue &&
                source.Details != null && source.Details.Contains(DetailsFieldBeginning);
    }

    private bool IsSuccessfullyRetrieved(AuditRecord source) =>
        source.Details != null && source.Details.EndsWith(DetailsFieldEnding);

    private string GetSectionValueFromAuditRecordDetails(string details)
    {
        string removedBeginning = details.Replace(DetailsFieldBeginning, null, StringComparison.Ordinal);
        string section = removedBeginning.Replace(DetailsFieldEnding, null, StringComparison.Ordinal);

        return section.Any(char.IsLower) ? null : section;
    }
}
