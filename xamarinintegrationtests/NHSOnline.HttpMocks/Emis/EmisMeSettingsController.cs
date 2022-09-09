using System;
using Microsoft.AspNetCore.Mvc;
using NHSOnline.HttpMocks.Domain;

namespace NHSOnline.HttpMocks.Emis
{
    [Route("emis/me/settings")]
    public class EmisMeSettingsController: Controller
    {
        private readonly IPatients _patients;

        public EmisMeSettingsController(IPatients patients)
        {
            _patients = patients;
        }

        public IActionResult MeSettings([FromQuery] string? userPatientLinkToken)
        {

            if (!ModelState.IsValid ||
                userPatientLinkToken == null ||
                !userPatientLinkToken.Contains("linktoken_", StringComparison.InvariantCulture))
            {
                return BadRequest(ModelState);
            }

            var patientId = userPatientLinkToken.Replace("linktoken_", string.Empty, StringComparison.InvariantCulture);
            var patient = _patients.LookupById(patientId);

            if (patient is EmisPatient emisPatient && !patient.PersonalDetails.Name.GivenName.Contains("Proxy", StringComparison.InvariantCulture))
            {
                var behaviour = emisPatient.Behaviours.Get<IEmisMeSettingsBehaviour>(() => new EmisMeSettingsDefaultBehaviour());
                return behaviour.Behave(emisPatient);
            }

            if (patient is EmisPatient emisPatientLinkedProfile && patient.PersonalDetails.Name.GivenName.Contains("Proxy", StringComparison.InvariantCulture))
            {
                var behaviour = emisPatientLinkedProfile.Behaviours.Get<IEmisMeSettingsLinkedProfileBehaviour>(() => new EmisMeSettingsLinkedProfileBehaviour());
                return behaviour.Behave(emisPatientLinkedProfile);
            }

            return BadRequest($"Patent '{patientId}' ({patient?.GetType().Name ?? "Unknown"}) is not an EMIS patient");
        }
    }
}