using NHSOnline.HttpMocks.Domain;

namespace NHSOnline.HttpMocks.Ndop
{
    public static class PatientExtensions
    {
        public static TPatient WithBehaviour<TPatient>(this TPatient patient, INdopCreateSessionBehaviour behaviour)
            where TPatient : Patient
            => patient.WithBehaviour<TPatient, INdopCreateSessionBehaviour>(behaviour);
    }
}