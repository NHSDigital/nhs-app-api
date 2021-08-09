using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using NHSOnline.HttpMocks.Domain;

namespace NHSOnline.HttpMocks.CitizenId
{
    public interface INhsLoginSSOBehaviour
    {
        [SuppressMessage("Design", "CA1054: URI parameters should not be strings", Justification = "Parameter is passed through so no need to parse to URI")]
        public Task<IActionResult> Behave(string state, string scope, Patient patient, string redirectUri);
    }
}