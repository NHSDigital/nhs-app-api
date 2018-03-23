using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace NHSOnline.Backend.Worker.Filters
{
    public class TimeoutExceptionFilter : ExceptionFilterAttribute
    {
        public override void OnException(ExceptionContext context)
        {
            if (context.Exception is TaskCanceledException)
            {
                context.Result = new StatusCodeResult(StatusCodes.Status504GatewayTimeout);
            }
        }
    }
}
