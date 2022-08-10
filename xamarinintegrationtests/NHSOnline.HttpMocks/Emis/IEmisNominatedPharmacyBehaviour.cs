using Microsoft.AspNetCore.Mvc;
using NHSOnline.HttpMocks.Domain;

namespace NHSOnline.HttpMocks.Emis
{
    public interface IEmisNominatedPharmacyBehaviour
    {
        IActionResult Behave(EmisPatient patient);
    }
}

