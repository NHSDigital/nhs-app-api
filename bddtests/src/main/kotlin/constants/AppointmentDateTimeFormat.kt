package constants

class AppointmentDateTimeFormat {
    companion object {
        const val backendDateTimeFormatWithoutTimezone = "yyyy-MM-dd'T'HH:mm:ss"
        const val backendDateTimeFormatWithTimezone = "yyyy-MM-dd'T'HH:mm:ssZ"
        const val frontendDateFormat = "EEEE d MMMM yyyy"
        const val frontendTimeFormat = "h:mm a"
    }
}