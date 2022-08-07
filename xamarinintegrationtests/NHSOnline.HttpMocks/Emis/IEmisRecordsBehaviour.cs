using Microsoft.AspNetCore.Mvc;

namespace NHSOnline.HttpMocks.Emis
{
    public interface IEmisRecordsBehaviour
    {
        IActionResult Behave();
    }
}