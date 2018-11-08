package constants

object ErrorResponseCodeEmis {
    const val UNKNOWN_EXCEPTION: Long = -9999
    const val INTERNAL_ERROR: Long = -1002
    const val SERVICE_ACCESS_VIOLATION: Long = -1030
    const val NOT_AVAILABLE: Long = -1151
    const val NO_REGISTERED_ONLINE_USER_FOUND: Long = -1104
    const val ACCOUNT_STATUS_INVALID: Long = -1107
    const val REQUESTED_APPOINTMENT_SLOT_IN_PAST: Long = -1152
    const val PRACTICE_NOT_LIVE: Long = -1401
    const val ALREADY_PENDING_REQUEST: Long = -1455
    const val PATIENT_NOT_REGISTERED_AT_PRACTICE: Long = -1551
    const val PATIENT_MARKED_AS_ARCHIVED: Long = -1552
    const val PATIENT_NON_COMPETENT_OR_UNDER_16: Long = -1553
    const val ONLINE_USER_MAX_APPOINTMENT_BOOKED_COUNT: Long = -1156
}
