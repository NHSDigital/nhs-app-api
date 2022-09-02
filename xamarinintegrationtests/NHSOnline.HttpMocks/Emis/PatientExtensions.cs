using NHSOnline.HttpMocks.Domain;
using NHSOnline.HttpMocks.Tpp;
using NHSOnline.HttpMocks.Tpp.Models;

namespace NHSOnline.HttpMocks.Emis
{
    public static class PatientExtensions
    {
        public static TPatient WithBehaviour<TPatient>(this TPatient patient, IEmisCreateSessionBehaviour behaviour)
            where TPatient : Patient
            => patient.WithBehaviour<TPatient, IEmisCreateSessionBehaviour>(behaviour);

        public static TPatient WithBehaviour<TPatient>(this TPatient patient, IEmisRecordsBehaviour behaviour)
            where TPatient : Patient
            => patient.WithBehaviour<TPatient, IEmisRecordsBehaviour>(behaviour);

        public static TPatient WithBehaviour<TPatient>(this TPatient patient, ITppRecordsBehaviour behaviour)
            where TPatient : Patient
            => patient.WithBehaviour<TPatient, ITppRecordsBehaviour>(behaviour);
    }
}