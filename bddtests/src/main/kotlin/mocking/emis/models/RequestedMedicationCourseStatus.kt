package mocking.emis.models

enum class RequestedMedicationCourseStatus(val requestedMedicationCourseStatus: Int)
{
    Issued(1),
    Requested(2),
    ForwardedForSigning(3),
    Rejected (4),
    Unknown(5),
    Cancelled(6),
}
