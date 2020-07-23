using Microsoft.AspNetCore.Mvc;
using NHSOnline.Backend.Support;

namespace NHSOnline.Backend.PfsApi.Areas.Session
{
    public interface ISessionErrorResultBuilder
    {
        IActionResult BuildResult(ErrorTypes errorTypes);
    }
}