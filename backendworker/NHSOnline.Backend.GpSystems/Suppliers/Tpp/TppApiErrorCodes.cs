namespace NHSOnline.Backend.GpSystems.Suppliers.Tpp
{
    public static class TppApiErrorCodes
    {
        public const string NotAuthenticated = "3";
        public const string StartDateInPast = "5";
        public const string NoAccess = "6";
        public const string AppointmentLimitReached = "7";
        public const string ProblemLoggingOn = "9";
        public const string AppointmentWithinOneHour = "40";
        public const string SlotNotFound = "1102";
        public const string SlotAlreadyBooked = "1103";
    }
}
