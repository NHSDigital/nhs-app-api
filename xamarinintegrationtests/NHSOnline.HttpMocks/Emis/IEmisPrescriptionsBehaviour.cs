using Microsoft.AspNetCore.Mvc;

namespace NHSOnline.HttpMocks.Emis
{
    public interface IEmisPrescriptionsBehaviour
    {
        IActionResult Behave();
    }
}