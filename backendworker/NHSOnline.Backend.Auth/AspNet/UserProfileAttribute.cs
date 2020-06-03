using System;

namespace NHSOnline.Backend.Auth.AspNet
{
    [AttributeUsage(AttributeTargets.Parameter)]
    public sealed class UserProfileAttribute: Attribute
    { }
}