using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;

namespace NHSOnline.Backend.ClinicalDecisionSupportApi.Settings
{
    public class SettingValidationStartupFilter : IStartupFilter
    {
        private readonly IEnumerable<IValidatable> _validatableObjects;

        public SettingValidationStartupFilter(IEnumerable<IValidatable> validatableObjects)
        {
            _validatableObjects = validatableObjects;
        }

        public Action<IApplicationBuilder> Configure(Action<IApplicationBuilder> next)
        {
            foreach (var validatableObject in _validatableObjects)
            {
                validatableObject.Validate();
            }

            return next;
        }
    }
}