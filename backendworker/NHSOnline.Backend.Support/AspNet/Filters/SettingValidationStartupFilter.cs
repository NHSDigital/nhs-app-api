using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;

namespace NHSOnline.Backend.Support.AspNet.Filters
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