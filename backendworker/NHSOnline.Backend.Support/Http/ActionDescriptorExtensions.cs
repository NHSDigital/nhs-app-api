using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Controllers;

namespace NHSOnline.Backend.Support.Http
{
    public static class ActionDescriptorExtensions
    {
        public static IDictionary<string, TAttributeType>
            GetParametersWithAttributeAndType<TAttributeType, TParameterType>(this ActionDescriptor descriptor)
            where TAttributeType : Attribute
        {
            var attributeType = typeof(TAttributeType);
            var parameterType = typeof(TParameterType);

            return descriptor?.Parameters
                .OfType<ControllerParameterDescriptor>()
                .Where(p =>
                    p.ParameterType == parameterType && p.HasAttribute<TAttributeType>())
                .ToDictionary(p => p.Name,
                    p =>
                        (TAttributeType) Attribute.GetCustomAttribute(p.ParameterInfo, attributeType))
                   ?? new Dictionary<string, TAttributeType>();
        }
    }
}
