using System.Linq;
﻿namespace NHSOnline.Backend.PfsApi.OrganDonation.ApiModels
{
    public class Issue
    {
        public string Code { get; set; }

        public CodeableConcept Details { get; set; }

        public string Diagnostics { get; set; }

        public override string ToString()
        {
            var firstDetails = Details?.Coding?.FirstOrDefault();

            return $"Error Code: '{Code}'. " +
                   $"FHIR Code: '{firstDetails?.Code}'. " +
                   $"FHIR Display: '{firstDetails?.Display}'. ";
        }
    }
}