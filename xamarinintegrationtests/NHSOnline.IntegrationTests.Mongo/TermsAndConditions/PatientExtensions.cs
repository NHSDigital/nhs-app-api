using System;
using NHSOnline.HttpMocks.Domain;

namespace NHSOnline.IntegrationTests.Mongo.TermsAndConditions
{
    public static class PatientExtensions
    {
        public static TPatient WithTermsAndConditionsAccepted<TPatient>(this TPatient patient, Func<ConsentRecord, ConsentRecord>? modifiers = null) where TPatient : Patient
        {
            var consentRecord = patient.ToConsent();
            if (modifiers != null)
            {
                consentRecord = modifiers(consentRecord);
            }
            ConsentCollection.Add(consentRecord);

            return patient;
        }
    }
}