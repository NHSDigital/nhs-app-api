using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using NHSOnline.HttpMocks.Domain;

namespace NHSOnline.HttpMocks.CitizenId
{
    public class NhsLoginSlowResponseSSOBehaviour : INhsLoginSSOBehaviour
    {
        private readonly TimeSpan _delay;

        public NhsLoginSlowResponseSSOBehaviour(TimeSpan delay)
        {
            _delay = delay;
        }

        [SuppressMessage("Design", "CA1054: URI parameters should not be strings", Justification = "Parameter is passed through so no need to parse to URI")]
        public async Task<IActionResult> Behave(string state, string scope, Patient patient, string redirectUri)
        {
            await Task.Delay(_delay);

            return new RedirectResult($"authorize?state={state}&scope={scope}&redirect_uri={redirectUri}");
        }
    }
}