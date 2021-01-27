using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using NHSOnline.HttpMocks.Domain;

namespace NHSOnline.HttpMocks.CitizenId
{
    public class NhsLoginAuthoriseBlankCodeBehaviour : INhsLoginAuthoriseBehaviour
    {
        public Task<IActionResult> Behave(Patient patient, string redirect, string state)
        {
            return Task.FromResult<IActionResult>(new RedirectResult($"{redirect}?state={state}&code="));
        }
    }
}