using System;
using NHSOnline.Backend.PfsApi.Filters;

namespace NHSOnline.Backend.PfsApi.Session
{
    /// <summary>
    /// Marker attribute for controller action parameters which take a specific user session. If the current
    /// session is not of the required type the request will be rejected as unauthorized.
    /// 
    /// See <see cref="UserSessionFilter"/> for its use.
    /// </summary>
    [AttributeUsage(AttributeTargets.Parameter)]
    public sealed class UserSessionAttribute: Attribute
    { }
}