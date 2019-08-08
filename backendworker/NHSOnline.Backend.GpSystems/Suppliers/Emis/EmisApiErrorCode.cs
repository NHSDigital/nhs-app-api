namespace NHSOnline.Backend.GpSystems.Suppliers.Emis
{
    public enum EmisApiErrorCode
    {
        //Appointments
        None = 0,
        RequiredFieldValueMissing = -1014,
        ProvidedAppointmentSlotInPast = -1152,
        AppointmentSlotIsBeforePracticeDefinedDays = -1153,
        AppointmentSlotIsAfterPracticeDefinedDays = -1154,
        OnlineUserMaxAppointmentBookCount = -1156
    }
}