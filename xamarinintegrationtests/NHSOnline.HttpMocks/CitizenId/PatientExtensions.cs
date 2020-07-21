using NHSOnline.HttpMocks.Domain;

namespace NHSOnline.HttpMocks.CitizenId
{
    public static class PatientExtensions
    {
        public static TPatient WithBehaviour<TPatient>(this TPatient patient, INhsLoginAuthoriseBehaviour behaviour)
            where TPatient : Patient
            => patient.WithBehaviour<TPatient, INhsLoginAuthoriseBehaviour>(behaviour);

        public static TPatient WithBehaviour<TPatient>(this TPatient patient, INhsLoginGetUserProfileBehaviour behaviour)
            where TPatient : Patient
            => patient.WithBehaviour<TPatient, INhsLoginGetUserProfileBehaviour>(behaviour);

        public static TPatient WithBehaviour<TPatient>(this TPatient patient, INhsLoginTokenBehaviour behaviour)
            where TPatient : Patient
            => patient.WithBehaviour<TPatient, INhsLoginTokenBehaviour>(behaviour);
    }
}