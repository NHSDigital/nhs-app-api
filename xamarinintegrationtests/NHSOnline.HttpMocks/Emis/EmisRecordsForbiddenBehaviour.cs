using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace NHSOnline.HttpMocks.Emis
{
    public class EmisRecordsForbiddenBehaviour : IEmisRecordsBehaviour
    {
        public IActionResult Behave() => new StatusCodeResult(StatusCodes.Status403Forbidden);
    }
}