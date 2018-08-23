package mocking.emis.models

enum class RequestedMedicationCourseStatus
{
    Issued,
    Requested,
    ForwardedForSigning,
    Rejected,
    Unknown,
    Cancelled
}
