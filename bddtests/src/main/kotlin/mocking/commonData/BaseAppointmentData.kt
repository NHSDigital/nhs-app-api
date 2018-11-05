package mocking.commonData

import addDays
import addHours
import addMinutes
import mocking.emis.models.AppointmentCancellationReason
import models.Patient
import java.text.SimpleDateFormat
import java.util.*

abstract class BaseAppointmentData {
    abstract val dateTimeFormat: SimpleDateFormat
    abstract val defaultPatient: Patient

    abstract fun getAppointmentCancellationReasons(): List<AppointmentCancellationReason>?

    protected fun copyCalendarDate(baseTime: Calendar, addDays: Int = 0,
                                   addHours: Int = 0, addMinutes: Int = 0) : Calendar {
        val theStartTime = baseTime.clone() as Calendar
        val numberOfMinutesToNext = roundMinutes - (theStartTime.get(Calendar.MINUTE) % roundMinutes)

        return theStartTime.addDays(addDays).addHours(addHours).addMinutes(addMinutes + numberOfMinutesToNext)
    }

    companion object {
        const val roundMinutes = 5
    }
}