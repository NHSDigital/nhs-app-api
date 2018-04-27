namespace NHSOnline.Backend.Worker.Bridges.Emis.Models.Prescriptions
{
    public enum RequestedMedicationCourseStatus
    {
        Issued,
        Requested,
        ForwardedForSigning,
        Rejected,
        Unknown,
        Cancelled,
    }
}
