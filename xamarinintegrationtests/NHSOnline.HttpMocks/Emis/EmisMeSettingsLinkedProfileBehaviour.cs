using Microsoft.AspNetCore.Mvc;
using NHSOnline.HttpMocks.Domain;

namespace NHSOnline.HttpMocks.Emis
{
    public class EmisMeSettingsLinkedProfileBehaviour: IEmisMeSettingsLinkedProfileBehaviour
    {
        public IActionResult Behave(EmisPatient patient)
        {
            return new JsonResult(new
            {
                AssignedServices = new {
                    AppointmentsEnabled = true,
                    PrescribingEnabled = false,
                    MedicalRecordEnabled = true
                }
            });
        }
    }
}