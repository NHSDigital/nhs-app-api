using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using NHSOnline.HttpMocks.Domain;

namespace NHSOnline.HttpMocks.CitizenId
{
    public interface INhsLoginAuthoriseBehaviour
    {
        public Task<IActionResult> Behave(Patient patient, string redirect, string state);
    }
}