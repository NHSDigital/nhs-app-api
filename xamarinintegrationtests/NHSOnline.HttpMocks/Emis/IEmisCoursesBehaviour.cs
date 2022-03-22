using Microsoft.AspNetCore.Mvc;
using NHSOnline.HttpMocks.Domain;

namespace NHSOnline.HttpMocks.Emis
{
    public interface IEmisCoursesBehaviour
    {
        IActionResult Behave();
    }
}