using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Configuration;

namespace NHSOnline.Backend.Worker.Filters
{
    public class SecurityModeFilter : IActionFilter
    {
        private readonly RunMode _runMode;

        public SecurityModeFilter(IConfiguration configuration)
        {
            if (configuration["runMode"] != null)
            {
                var stringMode = configuration["runMode"];

                if (!Enum.TryParse(stringMode, true, out _runMode))
                {
                    _runMode = RunMode.None;
                }
            }
        }

        public void OnActionExecuted(ActionExecutedContext context) {}

        public void OnActionExecuting(ActionExecutingContext context)
        {
            var securityAttribute = context.ActionDescriptor.FilterDescriptors
                .Select(x => x.Filter).OfType<SecurityModeAttribute>().FirstOrDefault();

            if (securityAttribute != null)
            {
                if (securityAttribute.SecurityMode == _runMode || _runMode == RunMode.Dev)
                    return;
            }
            
            context.Result = new NotFoundResult();
        }
    }
}