using NHSOnline.HttpMocks.Domain;

namespace NHSOnline.HttpMocks.CitizenId
{
    public static class PatientExtensions
    {
        public static TPatient WithBehaviour<TPatient>(this TPatient patient, INhsLoginAuthoriseBehaviour behaviour)
            where TPatient : Patient
            => patient.WithBehaviour<TPatient, INhsLoginAuthoriseBehaviour>(behaviour);
    }
}