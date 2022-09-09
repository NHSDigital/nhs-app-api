using System;

namespace NHSOnline.HttpMocks.Domain
{
    public static class PatientExtensions
    {
        public static TPatient WithLogin<TPatient>(this TPatient patient, string login) where TPatient: Patient
        {
            patient = patient ?? throw new ArgumentNullException(nameof(patient));

            patient.Login = login;
            return patient;
        }

        public static TPatient WithName<TPatient>(this TPatient patient, Func<PatientNameBuilder, PatientNameBuilder> build) where TPatient: Patient
        {
            patient = patient ?? throw new ArgumentNullException(nameof(patient));
            build = build ?? throw new ArgumentNullException(nameof(build));

            patient.PersonalDetails.Name = build(new PatientNameBuilder()).Build();
            return patient;
        }

        public static TPatient WithLinkedProfileName<TPatient>(this TPatient patient, TPatient linkedProfile) where TPatient: Patient
        {
            patient = patient ?? throw new ArgumentNullException(nameof(patient));
            patient.LinkedProfiles.Add(linkedProfile);
            return patient;
        }

        public static TPatient WithAge<TPatient>(this TPatient patient, int years, int days) where TPatient : Patient
        {
            patient = patient ?? throw new ArgumentNullException(nameof(patient));

            patient.PersonalDetails.Age = new PatientAge(years, days);
            return patient;
        }

        public static TPatient WithNhsNumber<TPatient>(this TPatient patient, NhsNumber nhsNumber)
            where TPatient : Patient
        {
            patient = patient ?? throw new ArgumentNullException(nameof(patient));

            patient.NhsNumber = nhsNumber;
            return patient;
        }

        public static TPatient WithNhsNumber<TPatient>(this TPatient patient, string nhsNumber)
            where TPatient : Patient
        {
            patient = patient ?? throw new ArgumentNullException(nameof(patient));

            patient.NhsNumber = NhsNumber.FromString(nhsNumber);
            return patient;
        }

        public static TPatient WithProofLevel5<TPatient>(this TPatient patient) where TPatient : Patient
        {
            patient = patient ?? throw new ArgumentNullException(nameof(patient));

            patient.ProofingLevel = "P5";
            patient.VectorOfTrust = "P5.Cp.Cd";
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