using Microsoft.AspNetCore.Mvc;
using NHSOnline.HttpMocks.Domain;

namespace NHSOnline.HttpMocks.Emis
{
    public interface IEmisMeSettingsLinkedProfileBehaviour
    {
        IActionResult Behave(EmisPatient patient);
    }
}