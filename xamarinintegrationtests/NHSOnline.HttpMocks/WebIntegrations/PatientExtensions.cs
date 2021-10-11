using NHSOnline.HttpMocks.Domain;

namespace NHSOnline.HttpMocks.WebIntegrations
{
    public static class PatientExtensions
    {
        public static TPatient WithBehaviour<TPatient>(this TPatient patient, IVaccineRecordSsoBehaviour behaviour)
            where TPatient : Patient
            => patient.WithBehaviour<TPatient, IVaccineRecordSsoBehaviour>(behaviour);
    }
}