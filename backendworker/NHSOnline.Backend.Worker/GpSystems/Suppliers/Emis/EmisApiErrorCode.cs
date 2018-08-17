namespace NHSOnline.Backend.Worker.GpSystems.Suppliers.Emis
{
    public enum EmisApiErrorCode
    {
        None = 0,
        FieldValueOutOfRange = -1017,
        ApiCommandNotPermitted = -1030,
        NoRegisteredOnlineUserFound = -1104,
        AccountStatusInvalid = -1107,
        PatientNotRegisteredAtPractice = -1551,
        ProvidedAppointmentSlotInPast = -1152,
        OnlineUserMaxAppointmentBookCount = -1156,
        PracticeNotLive = -1401,
        PatientMarkedAsArchived = -1552,
        PatientNonCompetentOrUnder16 = -1553,
        
    }
}
