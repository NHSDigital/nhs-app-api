using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Builder;

namespace NHSOnline.Backend.Support
{
    public class SettingValidationStartupFilter : IStartupFilter 
    {
        readonly IEnumerable<IValidateable> _validateableObjects;
        public SettingValidationStartupFilter(IEnumerable<IValidateable> validateableObjects)
        {
            _validateableObjects = validateableObjects;
        }

        public Action<IApplicationBuilder> Configure(Action<IApplicationBuilder> next)
        {
            foreach (var validateableObject in _validateableObjects)
            {
                validateableObject.Validate();
            }

            return next;
        }
    }
}