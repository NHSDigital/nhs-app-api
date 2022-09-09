using Microsoft.AspNetCore.Mvc;
using NHSOnline.HttpMocks.Domain;

namespace NHSOnline.HttpMocks.Emis
{
    public class EmisMeSettingsDefaultBehaviour: IEmisMeSettingsBehaviour
    {
        public IActionResult Behave(EmisPatient patient)
        {
            return new JsonResult(new
            {
                AssignedServices = new {
                    AppointmentsEnabled = true,
                    PrescribingEnabled = true,
                    MedicalRecordEnabled = true
                }
            });
        }
    }
}