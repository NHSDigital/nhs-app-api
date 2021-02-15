using System;

namespace NHSOnline.Backend.Support.Session
{
    public static class P9UserSessionExtensions
    {
        public static GpLinkedAccountModel BuildGpLinkedAccountModel(
            this P9UserSession p9UserSession,
            Guid patientSessionIdentifier)
        {
            if (p9UserSession.PatientLookup.TryGetValue(patientSessionIdentifier, out string value))
            {
                return new GpLinkedAccountModel(
                    p9UserSession.GpUserSession, value
                );
            }

            throw new InvalidPatientIdException();
        }

        public static bool TryGetPatientGpIdentifierFromSessionIdentifier(
            this P9UserSession p9UserSession,
            Guid patientSessionIdentifier,
            out string patientGpIdentifier)
        {
            if (p9UserSession.PatientLookup.TryGetValue(patientSessionIdentifier, out string value))
            {
                patientGpIdentifier = value;
                return true;
            }

            patientGpIdentifier = string.Empty;
            return false;
        }
    }
}