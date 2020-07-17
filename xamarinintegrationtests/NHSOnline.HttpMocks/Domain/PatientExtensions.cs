using System;

namespace NHSOnline.HttpMocks.Domain
{
    public static class PatientExtensions
    {
        public static TPatient WithId<TPatient>(this TPatient patient, string id) where TPatient: Patient
        {
            patient = patient ?? throw new ArgumentNullException(nameof(patient));

            patient.Id = id;
            return patient;
        }

        public static TPatient WithName<TPatient>(this TPatient patient, Func<PatientNameBuilder, PatientNameBuilder> build) where TPatient: Patient
        {
            patient = patient ?? throw new ArgumentNullException(nameof(patient));
            build = build ?? throw new ArgumentNullException(nameof(build));

            patient.PersonalDetails.Name = build(new PatientNameBuilder()).Build();
            return patient;
        }

        public static TPatient WithNhsNumber<TPatient>(this TPatient patient, NhsNumber nhsNumber) where TPatient : Patient
        {
            patient = patient ?? throw new ArgumentNullException(nameof(patient));

            patient.NhsNumber = nhsNumber;
            return patient;
        }

        internal static TPatient WithBehaviour<TPatient, TBehaviour>(this TPatient patient, TBehaviour behaviour)
            where TPatient : Patient
            where TBehaviour: class
        {
            patient = patient ?? throw new ArgumentNullException(nameof(patient));

            patient.Behaviours.Add(behaviour);
            return patient;
        }
    }
}