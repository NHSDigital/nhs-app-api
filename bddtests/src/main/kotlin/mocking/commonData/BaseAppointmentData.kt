package mocking.commonData

import addDays
import addHours
import addMinutes
import constants.AppointmentDateTimeFormat
import mocking.emis.models.AppointmentCancellationReason
import models.Patient
import models.Slot
import java.text.SimpleDateFormat
import java.util.*

abstract class BaseAppointmentData {
    abstract val dateTimeFormat: SimpleDateFormat
    abstract val defaultPatient: Patient

    abstract fun generateExpectedMyAppointments(timezone: String): ArrayList<Slot>

    abstract fun getAppointmentCancellationReasons(): List<AppointmentCancellationReason>?

    protected fun copyCalendarDate(baseTime: Calendar, addDays: Int = 0, addHours: Int = 0, addMinutes: Int = 0): Calendar {
        val theStartTime = baseTime.clone() as Calendar
        val numberOfMinutesToNextDivisibleByFive = 5 - (theStartTime.get(Calendar.MINUTE) % 5)
        return theStartTime.addDays(addDays).addHours(addHours).addMinutes(addMinutes + numberOfMinutesToNextDivisibleByFive)
    }

    protected fun convertToBrowserTimezone(time: String, timezone: String): String {
        val dateFormatWithUtcTimeZone = SimpleDateFormat(AppointmentDateTimeFormat.backendDateTimeFormatWithTimezone)
        dateFormatWithUtcTimeZone.timeZone = TimeZone.getTimeZone(timezone)
        val browserDate = dateTimeFormat.parse(time)
        return dateFormatWithUtcTimeZone.format(browserDate)
    }
}