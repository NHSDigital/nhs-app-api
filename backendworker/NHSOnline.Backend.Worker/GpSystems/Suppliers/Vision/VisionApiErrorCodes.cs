namespace NHSOnline.Backend.Worker.GpSystems.Suppliers.Vision
{
    public static class VisionApiErrorCodes
    {
        public const string AccessDenied = "-35";
        public const string AccountAlreadyRegistered = "-2";
        public const string AccountLocked = "-15";
        public const string AppointmentBookingLimitReached = "-25";
        public const string AppointmentSlotAlreadyBooked = "-100";
        public const string AppointmentSlotNotBookedToCurrentUser = "-100";
        public const string AppointmentSlotNotFound = "-21";
        public const string InvalidUserCredentials = "-30";
        public const string InvalidDetails = "-33";
        public const string InvalidParameter = "-31";
        public const string UnknownError = "-100";

        // Linkage
        public const string InvalidNhsNumber = "V4205";
        public const string LinkageKeyRevoked = "TO_BE_CONFIRMED"; // to be confirmed - Vision question tracker #28
        public const string PatientRecordNotFound = "VY806";
        public const string LinkageKeyAlreadyExists = "V2214";
    }
}
