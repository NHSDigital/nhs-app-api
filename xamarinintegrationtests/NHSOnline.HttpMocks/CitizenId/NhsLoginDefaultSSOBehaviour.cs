using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using NHSOnline.HttpMocks.Domain;

namespace NHSOnline.HttpMocks.CitizenId
{
    public class NhsLoginDefaultSSOBehaviour : INhsLoginSSOBehaviour
    {
        [SuppressMessage("Design", "CA1054: URI parameters should not be strings", Justification = "Parameter is passed through so no need to parse to URI")]
        public Task<IActionResult> Behave(string state, string scope, Patient patient, string redirectUri)
        {
            return Task.FromResult<IActionResult>(new RedirectResult($"complete-login?state={state}&scope={scope}&patientId={patient.Id}&redirect_uri={redirectUri}"));
        }
    }
}