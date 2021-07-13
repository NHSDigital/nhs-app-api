using System;
using NHSOnline.HttpMocks.Domain;

namespace NHSOnline.IntegrationTests.Logs
{
    public class InstallReferrerLogs : DockerLogs
    {
        private static string BuildRegexString(Patient patient, string odsCode) =>
            $"OdsCode={odsCode} Action=Login NhsLoginId={patient.Id} ProofLevel={patient.ProofingLevel}.+ Referrer=utm_source%3dgoogle-play%26utm_medium%3dorganic";

        private InstallReferrerLogs(DateTime startTime, DateTime endTime, string containerName, Patient patient, string odsCode) :
            base(startTime, endTime, containerName, BuildRegexString(patient, odsCode))
        {
        }

        public static InstallReferrerLogs GetLogs(DateTime startTime, DateTime endTime, string containerName, Patient patient, string odsCode)
            => new(startTime, endTime, containerName, patient, odsCode);
    }
}