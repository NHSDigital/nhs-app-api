using NHSOnline.HttpMocks.Domain;
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

        public static TPatient WithBehaviour<TPatient>(this TPatient patient, IEmisNominatedPharmacyBehaviour behaviour)
            where TPatient : Patient
            => patient.WithBehaviour<TPatient, IEmisNominatedPharmacyBehaviour>(behaviour);

        public static TPatient WithBehaviour<TPatient>(this TPatient patient, ITppRecordsBehaviour behaviour)
            where TPatient : Patient
            => patient.WithBehaviour<TPatient, ITppRecordsBehaviour>(behaviour);

        public static TPatient WithBehaviour<TPatient>(this TPatient patient, IEmisCoursesBehaviour behaviour)
            where TPatient : Patient
            => patient.WithBehaviour<TPatient, IEmisCoursesBehaviour>(behaviour);

        public static TPatient WithBehaviour<TPatient>(this TPatient patient, IEmisPrescriptionsBehaviour behaviour)
            where TPatient : Patient
            => patient.WithBehaviour<TPatient, IEmisPrescriptionsBehaviour>(behaviour);
    }
}