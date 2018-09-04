using System;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Options;
using NHSOnline.Backend.Worker.Settings;

namespace NHSOnline.Backend.Worker.Filters
{
    public abstract class SecurityModeAttribute : Attribute, IFilterMetadata
    {
        public RunMode SecurityMode { get; protected set; }
    }

    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
    public sealed class CidSecurityModeAttribute : SecurityModeAttribute
    {
        public CidSecurityModeAttribute()
        {
            SecurityMode = RunMode.Cid;
        }
    }
    
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
    public sealed class PfsSecurityModeAttribute : SecurityModeAttribute
    {
        public PfsSecurityModeAttribute()
        {
            SecurityMode = RunMode.Pfs;
        }
    }
}