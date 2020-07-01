using Microsoft.AspNetCore.Mvc;
using NHSOnline.Backend.PfsApi.Session;

namespace NHSOnline.Backend.PfsApi.Areas.Session
{
    internal sealed class DeleteUserSessionResponseVisitor : IDeleteUserSessionResultVisitor<IActionResult>
    {
        public IActionResult Visit(DeleteUserSessionResult.Success success) => new NoContentResult();

        // Even when the delete session fails, we still return a no content result, to ensure that the
        // user is logged out of the app and cookies are deleted.
        public IActionResult Visit(DeleteUserSessionResult.Failure failure) => new NoContentResult();
    }
}