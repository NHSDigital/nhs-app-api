using Microsoft.AspNetCore.Mvc;

namespace NHSOnline.HttpMocks.Spine
{
    public interface ISpineBehaviour
    {
        IActionResult Behave();
    }
}