using Microsoft.AspNetCore.Mvc;
using NHSOnline.HttpMocks.Domain;

namespace NHSOnline.HttpMocks.Emis
{
    public interface IEmisCreateSessionLinkedProfileBehaviour
    {
        IActionResult Behave(EmisPatient patient);
    }
}