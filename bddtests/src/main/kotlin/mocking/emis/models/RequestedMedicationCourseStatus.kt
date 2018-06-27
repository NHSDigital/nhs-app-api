package mocking.emis.models

enum class RequestedMedicationCourseStatus(val requestedMedicationCourseStatus: Int)
{
    Issued(0),
    Requested(1),
    ForwardedForSigning(2),
    Rejected (3),
    Unknown(4),
    Cancelled(5),
}
