using System;
using System.Text.RegularExpressions;

namespace NHSOnline.MetricLogFunctionApp.Etl.Functions.AuditLog.RegistrationAndLogin.LastLoginPatientIdentifier;
public class LastLoginPatientIdentifierEventParser: IAuditLogParser<LastLoginPatientIdentifier>
{
    private const string OperationFieldValue = "CitizenId_Session_Create_Request";
    private const string DetailsFieldValue = "Create Citizen Id Session";

    public LastLoginPatientIdentifier Parse(AuditRecord source)
    {
        if (!IsLastLoginPatientIdentifier(source)) return null;

        return new LastLoginPatientIdentifier
        {
            LoginId = source.NhsLoginSubject,
            NhsNumber = source.NhsNumber,
            Timestamp = source.Timestamp,
            AuditId = source.AuditId
        };
    }
    private bool IsLastLoginPatientIdentifier(AuditRecord source)
    {
        if (source == null) return false;

        return source.Operation == OperationFieldValue &&
               source.Details is DetailsFieldValue;
    }
}
