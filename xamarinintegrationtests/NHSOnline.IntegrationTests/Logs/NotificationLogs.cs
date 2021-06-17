using System;
using NHSOnline.HttpMocks.Domain;

namespace NHSOnline.IntegrationTests.Logs
{
    public class NotificationLogs : DockerLogs
    {
        private static string BuildRegexString(Patient patient, string odsCode, string outcome) =>
            $"Ods={odsCode} Action={outcome} NhsLoginId={patient.Id} ProofLevel={patient.VectorOfTrust}.+";

        private NotificationLogs(DateTime startTime, DateTime endTime, string containerName, Patient patient, string odsCode, string outcome) :
            base(startTime, endTime, containerName, BuildRegexString(patient, odsCode, outcome))
        {
        }

        public static NotificationLogs GetLogs(DateTime startTime, DateTime endTime, string containerName, Patient patient, string odsCode, string outcome)
            => new(startTime, endTime, containerName, patient, odsCode, outcome);
    }
}