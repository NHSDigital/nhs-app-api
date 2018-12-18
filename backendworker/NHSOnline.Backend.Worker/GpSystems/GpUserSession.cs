using System;

namespace NHSOnline.Backend.Worker.GpSystems
{
    [Serializable]
    public abstract class GpUserSession
    {
        public string NhsNumber { get; set; }
        
        public string OdsCode { get; set; }

        public abstract Supplier Supplier { get; }
    }
}