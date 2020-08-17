using System;

namespace NHSOnline.Backend.Auth.AspNet
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Parameter)]
    public sealed class UserProfileAttribute: Attribute
    { }
}