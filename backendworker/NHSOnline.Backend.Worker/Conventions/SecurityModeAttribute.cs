using System;

namespace NHSOnline.Backend.Worker.Conventions
{
    public abstract class SecurityModeAttribute : Attribute
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
    
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
    public sealed class AllSecurityModeAttribute : SecurityModeAttribute
    {
        public AllSecurityModeAttribute()
        {
            SecurityMode = RunMode.All;
        }
    }
}