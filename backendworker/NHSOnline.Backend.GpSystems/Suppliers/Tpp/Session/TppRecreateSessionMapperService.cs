using System;
using System.Linq;
using NHSOnline.Backend.GpSystems.Session;
using NHSOnline.Backend.Support;

namespace NHSOnline.Backend.GpSystems.Suppliers.Tpp.Session
{
    public class TppRecreateSessionMapperService : IRecreateSessionMapperService
    {
        public GpUserSession Map(GpUserSession gpUserSession, string suid, string patientId)
        {
            TppUserSession tppUserSession = (TppUserSession) gpUserSession;

            if (!IsValidPatientId(tppUserSession, patientId))
            {
                return gpUserSession;
            }

            if (tppUserSession.PatientId.Equals(patientId, StringComparison.Ordinal))
            {
                // moving from a proxy patient to main patient
                tppUserSession.Suid = suid;
                tppUserSession.ProxyPatients.ToList().ForEach(pp => pp.Suid = null);
            }
            else
            {
                // moving from the main patient to a proxy patient
                tppUserSession.Suid = null;
                tppUserSession.ProxyPatients.ToList().ForEach(pp =>
                    pp.Suid = pp.PatientId.Equals(patientId, StringComparison.Ordinal) ? suid : null);
            }
            return tppUserSession;
        }

        private static bool IsValidPatientId(TppUserSession tppUserSession, string patientId)
        {
            return tppUserSession.PatientId.Equals(patientId, StringComparison.Ordinal) ||
                   tppUserSession.ProxyPatients.ToList().Any(pp
                       => pp.PatientId.Equals(patientId, StringComparison.Ordinal));
        }
    }
}