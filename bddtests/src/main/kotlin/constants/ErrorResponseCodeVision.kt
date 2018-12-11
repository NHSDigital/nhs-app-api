package constants

object ErrorResponseCodeVision {
    const val ACCESS_DENIED = "-35"
    const val ACCOUNT_ALREADY_REGISTERED = "-2"
    const val ACCOUNT_LOCKED = "-15"
    const val APPOINTMENT_BOOKING_LIMIT_REACHED = "-25"
    const val APPOINTMENT_SLOT_NOT_BOOKED_TO_CURRENT_USER = "-100"
    const val APPOINTMENT_SLOT_NOT_FOUND = "-21"
    const val INVALID_DETAILS = "-33"
    const val INVALID_PARAMETERS = "-31"
    const val UNKNOWN_ERROR = "-100"

    const val NON_VISION_ERROR_CODE = "9999"

    // rest api error codes
    const val INVALID_NHS_NUMBER = "V4205"
    const val LINKAGE_KEY_REVOKED = "TO_BE_CONFIRMED" // to be confirmed - Vision question tracker #28
    const val PATIENT_RECORD_NOT_FOUND = "VY806"
    const val LINKAGE_KEY_ALREADY_EXISTS = "V2214"

}