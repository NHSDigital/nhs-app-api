package constants

object ErrorResponseCodeTpp {
    const val NOT_AUTHENTICATED = "3"
    const val LOGIN_PROBLEM = "9"
    const val NO_ACCESS = "6"
    const val SERVICE_UNAVAILABLE = "0"
    const val NOT_LOGGED_IN = "5"

    //Appointments
    const val START_DATE_IN_PAST = "5"
    const val APPOINTMENT_LIMIT_REACHED = "7"
    const val APPOINTMENT_WITHIN_ONE_HOUR = "40"
    const val SLOT_NOT_FOUND = "1102"
    const val SLOT_ALREADY_BOOKED = "1103"

    //Prescriptions
    const val MEDICATION_UNAVAILABLE = "1"

    const val UNKNOWN_ERROR = "0000"

    //Link Account constants
    const val INVALID_PROVIDER_ID = "5"
    const val INVALID_LINKAGE_CREDENTIALS = "8"

    //Documents
    const val FILE_SIZE_TOO_LARGE = "24"
    const val FILE_STILL_UPLOADING = "45"

}