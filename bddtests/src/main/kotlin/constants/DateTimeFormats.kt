package constants

class DateTimeFormats {
    companion object {
        const val backendDateTimeFormatWithoutTimezone = "yyyy-MM-dd'T'HH:mm:ss"
        const val backendDateTimeFormatWithTimezone = "yyyy-MM-dd'T'HH:mm:ssxxx"
        const val tppDateTimeFormat = "yyyy-MM-dd'T'HH:mm:ss.'0Z'"
        const val emisMessageDateTimeFormat = "yyyy-MM-dd'T'HH:mm:ss.SSS"
        const val frontendDateFormat = "EEEE d MMMM yyyy"
        const val frontendBasicDateFormat = "d MMMM yyyy"
        const val frontendTimeFormat = "h:mma"  // Warning: will need to convert resulting String toLowerCase()
        const val fullDayOfWeek = "EEEE"
        const val dateWithoutTimeFormat = "yyyy-MM-dd"
    }
}
