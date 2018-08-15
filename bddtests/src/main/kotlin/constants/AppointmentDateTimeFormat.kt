package constants

class AppointmentDateTimeFormat {
    companion object {
        const val backendDateTimeFormatWithoutTimezone = "yyyy-MM-dd'T'HH:mm:ss"
        const val backendDateTimeFormat = "yyyy-MM-dd'T'HH:mm:ss.'0Z'"
        const val tppDateTimeFormat = "yyyy-MM-dd'T'HH:mm:ss.'0Z'"
        const val frontendDateFormat = "EEEE d MMMM yyyy"
        const val frontendTimeFormat = "h:mma"
    }
}