using NHSOnline.HttpMocks.Domain;

namespace NHSOnline.HttpMocks.Emis
{
    public static class PatientExtensions
    {
        public static TPatient WithBehaviour<TPatient>(this TPatient patient, IEmisCreateSessionBehaviour behaviour)
            where TPatient : Patient
            => patient.WithBehaviour<TPatient, IEmisCreateSessionBehaviour>(behaviour);
    }
}