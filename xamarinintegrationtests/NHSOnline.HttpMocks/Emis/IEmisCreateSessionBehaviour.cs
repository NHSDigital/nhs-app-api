using Microsoft.AspNetCore.Mvc;
using NHSOnline.HttpMocks.Domain;

namespace NHSOnline.HttpMocks.Emis
{
    public interface IEmisCreateSessionBehaviour
    {
        IActionResult Behave(EmisPatient patient);
    }
}