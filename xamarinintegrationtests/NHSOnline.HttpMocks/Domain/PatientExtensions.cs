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
    }
}