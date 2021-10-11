using Microsoft.AspNetCore.Mvc;
using NHSOnline.HttpMocks.Domain;

namespace NHSOnline.HttpMocks.WebIntegrations
{
    public class RedirectToEnterEmailVaccineRecordSsoBehaviour : IVaccineRecordSsoBehaviour
    {
        private const string AuthHostName = "auth.nhslogin.stubs.local.bitraft.io";

        public IActionResult Behave(Patient patient)
        {
            return new RedirectResult($"http://{AuthHostName}:8080/citizenid/redirect-chain-1");
        }
    }
}