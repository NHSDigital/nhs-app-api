using Microsoft.AspNetCore.Mvc;
using NHSOnline.HttpMocks.Domain;

namespace NHSOnline.HttpMocks.WebIntegrations
{
    public class DefaultVaccineRecordSsoBehaviour : IVaccineRecordSsoBehaviour
    {
        public IActionResult Behave(Patient patient)
        {
            return new RedirectResult($"http://{VaccineRecordController.HostName}:8080/vaccine-record");
        }
    }
}