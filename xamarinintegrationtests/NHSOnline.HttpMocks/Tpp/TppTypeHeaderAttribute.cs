using System;
using Microsoft.AspNetCore.Mvc.ActionConstraints;

namespace NHSOnline.HttpMocks.Tpp
{
    [AttributeUsage(AttributeTargets.Method)]
    internal sealed class TppTypeHeaderAttribute : Attribute, IActionConstraint
    {
        public TppTypeHeaderAttribute(string value)
        {
            Value = value;
        }

        public string Value { get; }

        public bool Accept(ActionConstraintContext context)
        {
            if (context.RouteContext.HttpContext.Request.Headers.TryGetValue("type", out var value))
            {
                return value == Value;
            }

            return false;
        }

        public int Order => 0;
    }
}