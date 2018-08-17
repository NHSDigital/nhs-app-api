namespace NHSOnline.Backend.Worker.GpSystems.Suppliers.Emis
{
    public enum EmisApiErrorCode
    {
        None = 0,
        FieldValueOutOfRange = -1017,
        ApiCommandNotPermitted = -1030,
        ProvidedAppointmentSlotInPast = -1152,
        OnlineUserMaxAppointmentBookCount = -1156
    }
}
