using Microsoft.AspNetCore.Mvc;
using NHSOnline.HttpMocks.Domain;

namespace NHSOnline.HttpMocks.WebIntegrations
{
    public interface IVaccineRecordSsoBehaviour
    {
        public IActionResult Behave(Patient patient);
    }
}