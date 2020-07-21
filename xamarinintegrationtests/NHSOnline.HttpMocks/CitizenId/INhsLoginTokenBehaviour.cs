using Microsoft.AspNetCore.Mvc;
using NHSOnline.HttpMocks.Domain;

namespace NHSOnline.HttpMocks.CitizenId
{
    public interface INhsLoginTokenBehaviour
    {
        IActionResult Behave(Patient patient);
    }
}