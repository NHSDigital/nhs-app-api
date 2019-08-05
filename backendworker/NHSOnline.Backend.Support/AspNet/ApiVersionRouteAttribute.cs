using System;
using Microsoft.AspNetCore.Mvc;

namespace NHSOnline.Backend.Support.AspNet
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
    public sealed class ApiVersionRouteAttribute : RouteAttribute
    {
        public ApiVersionRouteAttribute(string template) : base($"v{{version:apiVersion}}/{template}")
        {
            
        }
    }
}