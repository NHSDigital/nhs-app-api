using Microsoft.AspNetCore.Mvc;
using NHSOnline.HttpMocks.Domain;

namespace NHSOnline.HttpMocks.CitizenId
{
    public interface INhsLoginGetUserProfileBehaviour
    {
        IActionResult Behave(Patient patient);
    }
}