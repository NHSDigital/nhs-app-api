using System.Linq;
using Microsoft.AspNetCore.Mvc.Controllers;

namespace NHSOnline.Backend.Support.Http
{
    public static class ControllerParameterExtensions
    {
        public static bool HasAttribute<T>(this ControllerParameterDescriptor descriptor)
        {
            var attributeType = typeof(T);

            return descriptor?.ParameterInfo
                .CustomAttributes
                .Any(a => a.AttributeType == attributeType) ?? false;
        }
    }
}