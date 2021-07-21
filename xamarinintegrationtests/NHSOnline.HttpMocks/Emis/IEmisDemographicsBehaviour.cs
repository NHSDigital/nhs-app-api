using Microsoft.AspNetCore.Mvc;
using NHSOnline.HttpMocks.Domain;

namespace NHSOnline.HttpMocks.Emis
{
    public interface IEmisDemographicsBehaviour
    {
        IActionResult Behave(EmisPatient patient);
    }
}