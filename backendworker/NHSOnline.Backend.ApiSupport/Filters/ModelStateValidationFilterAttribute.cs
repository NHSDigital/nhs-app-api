using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;

namespace NHSOnline.Backend.ApiSupport.Filters
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
    public sealed class ModelStateValidationFilterAttribute : ActionFilterAttribute
    {
        private readonly ILogger<ModelStateValidationFilterAttribute> _logger;

        public ILogger<ModelStateValidationFilterAttribute> Logger => _logger;   

        public ModelStateValidationFilterAttribute(ILogger<ModelStateValidationFilterAttribute> logger)
        {
            _logger = logger;
        }     

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (!context.ModelState.IsValid)
            {
                _logger.LogWarning("Model state validation failed: {0}", context.ModelState.Values.SelectMany(x=>x.Errors).Select(x=>x.ErrorMessage));
                context.Result = new BadRequestObjectResult(context.ModelState);
            }
        }

        public override void OnActionExecuted(ActionExecutedContext context) { }
    }
}
