using System;
using NHSOnline.Backend.PfsApi.Filters;

namespace NHSOnline.Backend.PfsApi.GpSession
{
    /// <summary>
    /// Marker attribute for controller action parameters which take a GP session. If the current
    /// gp session is invalid the API will attempt to re-establish the session before calling the
    /// endpoint.
    ///
    /// See: <see cref="GpSessionFilter"/>
    /// </summary>
    [AttributeUsage(AttributeTargets.Parameter)]
    public sealed class GpSessionAttribute : Attribute
    {
        public bool IgnoreP5Users { get; set; }
    }
}
