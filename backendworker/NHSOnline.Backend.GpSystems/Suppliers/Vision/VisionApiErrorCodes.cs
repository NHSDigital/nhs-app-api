namespace NHSOnline.Backend.GpSystems.Suppliers.Vision
{
    public static class VisionApiErrorCodes
    {
        public const string AccessDenied = "-35";
        public const string AppointmentBookingLimitReached = "-25";
        public const string AppointmentSlotAlreadyBooked = "-100";
        public const string AppointmentSlotNotBookedToCurrentUser = "-100";
        public const string AppointmentSlotNotFound = "-21";
        public const string InvalidUserCredentials = "-30";
        public const string UnknownError = "-100";
    }
}
