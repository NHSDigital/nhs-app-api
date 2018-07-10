using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NHSOnline.Backend.Worker.GpSystems.Suppliers.Tpp
{
    public static class TppApiErrorCodes
    {
        public const string StartDateInPast = "5";
        public const string NoAccess = "6";
        public const string AppointmentLimitReached = "7";
        public const string AppointmentWithinOneHour = "40";
        public const string SlotNotFound = "1102";
        public const string SlotAlreadyBooked = "1103";
    }
}
