using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.Extensions.Logging;

namespace NHSOnline.Backend.Worker
{
    public class SecurityModeConvention : IActionModelConvention
    {
        private readonly RunMode _runMode;
        private readonly ILogger<SecurityModeConvention> _logger;

        public SecurityModeConvention(RunMode runMode, ILogger<SecurityModeConvention> logger)
        {
            _runMode = runMode;
            _logger = logger;
        }
        public void Apply(ActionModel action)
        {
            if (_runMode == RunMode.None)
            {
                action.Selectors.Clear();
                _logger.LogInformation($"Runmode set to {_runMode}, removing routing for {action.DisplayName}");
                return;
            }
            
            var actionSecurityMode = FindSecurityMode(action.Attributes);

            if (actionSecurityMode == RunMode.None)
            {
                actionSecurityMode = action.Controller == null ? RunMode.None : FindSecurityMode(action.Controller.Attributes);
            }

            if (!ValidSecurityMode(actionSecurityMode))
            {
                action.Selectors.Clear();
                _logger.LogInformation($"Runmode set to {_runMode}, removing routing for {action.DisplayName}");
            }
        }

        private bool ValidSecurityMode(RunMode actionSecurityMode)
        {
            if (actionSecurityMode == RunMode.None)
            {
                return false;
            }

            return actionSecurityMode == _runMode || _runMode == RunMode.Dev || actionSecurityMode == RunMode.All;
        }

        private static RunMode FindSecurityMode(IEnumerable<object> attributeList)
        {
            var securityAttribute = attributeList.Select(x => x)
                .OfType<SecurityModeAttribute>().FirstOrDefault();

            return securityAttribute?.SecurityMode ?? RunMode.None;
        }
    }
}