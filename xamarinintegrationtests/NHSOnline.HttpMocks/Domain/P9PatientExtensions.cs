using System;

namespace NHSOnline.HttpMocks.Domain
{
    public static class P9PatientExtensions
    {

        public static TPatient WithUnknownSupplierOdsCode<TPatient>(this TPatient patient) where TPatient : P9Patient
        {
            patient = patient ?? throw new ArgumentNullException(nameof(patient));

            patient.OdsCode = "no-ods-code";
            return patient;
        }

        public static TPatient WithUnknownOdsCode<TPatient>(this TPatient patient) where TPatient : P9Patient
        {
            patient = patient ?? throw new ArgumentNullException(nameof(patient));

            patient.OdsCode = "Unknown";
            return patient;
        }
    }
}