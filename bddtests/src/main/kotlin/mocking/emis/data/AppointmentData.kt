package mocking.emis.data

import addDays
import addHours
import addMinutes
import constants.AppointmentDateTimeFormat.Companion.backendDateTimeFormatWithTimezone
import constants.AppointmentDateTimeFormat.Companion.backendDateTimeFormatWithoutTimezone
import constants.AppointmentDateTimeFormat.Companion.frontendDateFormat
import constants.AppointmentDateTimeFormat.Companion.frontendTimeFormat
import mocking.emis.appointments.GetAppointmentsResponseModel
import mocking.emis.models.*
import models.Slot
import java.text.SimpleDateFormat
import java.util.*
import kotlin.collections.ArrayList

private const val LOCATION_ID_SURGERY: Int = 1
private const val LOCATION_ID_HOSPITAL: Int = 2

private const val CLINICIAN_ID_DRSMITH: Int = 1
private const val CLINICIAN_ID_NURSEJONES = 2
private const val CLINICIAN_ID_MSBROWN = 3

private const val SESSION_ID_FOOTCLINIC = 1
private const val SESSION_ID_EYECLINIC = 2
private const val SESSION_ID_EARCLINIC = 3

class AppointmentData private constructor() {
    private val dateTimeFormat = SimpleDateFormat(backendDateTimeFormatWithoutTimezone)

    private val expectedMyAppointment = Slot(session = SessionType.Timed.toString())

    val locations = arrayListOf(
            Location(locationId = LOCATION_ID_SURGERY, locationName = "Main Surgery"),
            Location(locationId = LOCATION_ID_HOSPITAL, locationName = "Hospital")
    )

    val sessionHolders = arrayListOf(
            SessionHolder(clinicianId = CLINICIAN_ID_DRSMITH, displayName = "Dr. Smith"),
            SessionHolder(clinicianId = CLINICIAN_ID_NURSEJONES, displayName = "Nurse Jones"),
            SessionHolder(clinicianId = CLINICIAN_ID_MSBROWN, displayName = "Ms. Brown")
    )

    val sessions = arrayListOf(
            Session(sessionId = SESSION_ID_FOOTCLINIC,
                    sessionName = "Nose clinic",
                    sessionType = SessionType.Timed,
                    locationId = LOCATION_ID_SURGERY,
                    clinicianIds = arrayListOf(CLINICIAN_ID_DRSMITH)),
            Session(sessionId = SESSION_ID_EYECLINIC,
                    sessionName = "Eye clinic",
                    sessionType = SessionType.Timed,
                    locationId = LOCATION_ID_HOSPITAL,
                    clinicianIds = arrayListOf(CLINICIAN_ID_NURSEJONES, CLINICIAN_ID_MSBROWN)),
            Session(sessionId = SESSION_ID_EARCLINIC,
                    sessionName = "Ear clinic",
                    sessionType = SessionType.Timed,
                    locationId = LOCATION_ID_SURGERY,
                    clinicianIds = arrayListOf(CLINICIAN_ID_MSBROWN))
    )

    val telephoneAppointmentDetails1 =
        TelephoneAppointmentDetails(
                telephoneNumber = "0987654321",
                contactType = "Mobile")

    val telephoneAppointmentDetails2 =
        TelephoneAppointmentDetails(
                telephoneNumber = "0012345678",
                contactType = "Home")

    val emisCancellationReason1 =
        AppointmentCancellationReason("R1_NoLongerRequired", "No longer required")

    val emisCancellationReason2 =
        AppointmentCancellationReason("R2_UnableToAttend", "Unable to attend")

    private val unspecifiedTimeAppointment1 =
        Appointment(slotId = 1,
                sessionId = SESSION_ID_FOOTCLINIC,
                bookingReason = "My back hurts")

    private val unspecifiedTimeAppointment2 =
        Appointment(slotId = 2,
                sessionId = SESSION_ID_EARCLINIC,
                bookingReason = "My stomach hurts")

    private val unspecifiedTimeAppointment3 =
        Appointment(slotId = 3,
                sessionId = SESSION_ID_EYECLINIC,
                bookingReason = "My leg hurts")

    private var appointments: ArrayList<Appointment> = arrayListOf()

    fun createAppointmentSessions(): ArrayList<AppointmentSession> {
        val baseTime = Calendar.getInstance()

        var startTime = copyCalendarDate(baseTime, 1)
        var sessionDate = dateTimeFormat.format(startTime.time)
        val slot1 = createAppointmentSlot(1, startTime, 15)
        startTime.addDays(1).addMinutes(15)
        val slot2 = createAppointmentSlot(2, startTime, 1)
        val footClinicSession = AppointmentSession(
                sessionId = SESSION_ID_FOOTCLINIC,
                sessionDate = sessionDate,
                slots = arrayListOf(slot1, slot2)
        )

        startTime = copyCalendarDate(baseTime, 2)
        sessionDate = dateTimeFormat.format(startTime.time)
        val slot3 = createAppointmentSlot(3, startTime, 15)
        startTime.addDays(1)
        val slot4 = createAppointmentSlot(4, startTime, 20)
        val eyeClinicSession = AppointmentSession(
                sessionId = SESSION_ID_EYECLINIC,
                sessionDate = sessionDate,
                slots = arrayListOf(slot3, slot4)
        )

        startTime = copyCalendarDate(baseTime, 4)
        sessionDate = dateTimeFormat.format(startTime.time)
        val slot5 = createAppointmentSlot(3, startTime, 15)
        val earClinicSession = AppointmentSession(
                sessionId = SESSION_ID_EYECLINIC,
                sessionDate = sessionDate,
                slots = arrayListOf(slot5)
        )
        return arrayListOf(footClinicSession, eyeClinicSession, earClinicSession)
    }

    fun createGetAppointmentsResponse(): GetAppointmentsResponseModel {
        val baseDate = Calendar.getInstance()

        var bookingDate = copyCalendarDate(baseDate)
        val appointment1 = addDateToAppointment(unspecifiedTimeAppointment1.copy(), bookingDate, 1, 30)
        appointment1.telephoneAppointmentDetails = telephoneAppointmentDetails1

        bookingDate = copyCalendarDate(baseDate, addHours = 2)
        val appointment2 = addDateToAppointment(unspecifiedTimeAppointment2.copy(), bookingDate, 2, 20)
        appointment2.telephoneAppointmentDetails = telephoneAppointmentDetails2

        bookingDate = copyCalendarDate(baseDate, addHours = 2, addMinutes = 20)
        val appointment3 = addDateToAppointment(unspecifiedTimeAppointment3.copy(), bookingDate, 3, 15)
        appointment3.telephoneAppointmentDetails = telephoneAppointmentDetails1

        appointments = arrayListOf(appointment1, appointment2, appointment3)
        val appointmentsFromDate = appointment1.startTime

        return GetAppointmentsResponseModel(appointmentsFromDate, appointments, locations, sessionHolders, sessions)
    }

    fun getEmisAppointmentCancellationReasons(): List<AppointmentCancellationReason> {
        return arrayListOf(emisCancellationReason1, emisCancellationReason2)
    }

    fun createGetAppointmentsResponseForNoUpcomingAppointments(): GetAppointmentsResponseModel {
        val baseDate = Calendar.getInstance()
        val appointmentsFromDate = dateTimeFormat.format(baseDate.time)

        return GetAppointmentsResponseModel(appointmentsFromDate)
    }

    fun generateExpectedMyAppointments(timezone: String): ArrayList<Slot> {
        val expectedTempMyAppointments = arrayListOf<Slot>()
        val slotDateFormat = SimpleDateFormat(frontendDateFormat)
        val slotTimeFormat = SimpleDateFormat(frontendTimeFormat)
        appointments.forEach { appointment ->
            val frontendTime = convertToBrowserTimezone(appointment.startTime, timezone)
            val startDate = dateTimeFormat.parse(frontendTime)
            val date = slotDateFormat.format(startDate)
            val time = slotTimeFormat.format(startDate).toLowerCase()
            val location = locations[sessions[appointment.sessionId - 1].locationId!! - 1]
            val cliniciansNames: ArrayList<String> = ArrayList()
            val clinicianIds = sessions[appointment.sessionId - 1].clinicianIds
            clinicianIds.forEach { clinicianId ->
                cliniciansNames.add(sessionHolders[clinicianId - 1].displayName!!)
            }
            expectedTempMyAppointments.add(expectedMyAppointment.copy(
                    date = date,
                    time = time,
                    location = location.locationName!!,
                    clinician = cliniciansNames
            ))
        }

        return expectedTempMyAppointments
    }

    private fun addDateToAppointment(appointment: Appointment, bookingDate: Calendar, bookInDay: Int, durationInMinutes: Int): Appointment {
        appointment.bookingDate = dateTimeFormat.format(bookingDate.time)
        val startDate = bookingDate.addDays(bookInDay)
        appointment.startTime = dateTimeFormat.format(startDate.time)
        appointment.endTime = dateTimeFormat.format(startDate.addMinutes(durationInMinutes).time)
        return appointment
    }

    private fun copyCalendarDate(baseTime: Calendar, addDays: Int = 0, addHours: Int = 0, addMinutes: Int = 0): Calendar {
        val theStartTime = baseTime.clone() as Calendar
        val numberOfMinutesToNextDivisibleByFive = 5 - (theStartTime.get(Calendar.MINUTE) % 5)
        return theStartTime.addDays(addDays).addHours(addHours).addMinutes(addMinutes + numberOfMinutesToNextDivisibleByFive)
    }

    private fun createAppointmentSlot(sessionId: Int, startTime: Calendar, durationInMinutes: Int): AppointmentSlot {
        val startSession = dateTimeFormat.format(startTime.time)
        val endSession = dateTimeFormat.format(startTime.addMinutes(durationInMinutes).time)
        return AppointmentSlot(sessionId, startSession, endSession)
    }

    private class AppointmentDataHolder {
        private var instance: AppointmentData? = null

        fun getInstance(): AppointmentData {
            if (instance == null) {
                val newInstance = AppointmentData()
                instance = newInstance
            }
            return instance as AppointmentData
        }
    }

    private fun convertToBrowserTimezone(time: String, timezone: String): String {
        val dateFormatWithUtcTimeZone = SimpleDateFormat(backendDateTimeFormatWithTimezone)
        dateFormatWithUtcTimeZone.timeZone = TimeZone.getTimeZone(timezone)
        val browserDate = dateTimeFormat.parse(time)
        return dateFormatWithUtcTimeZone.format(browserDate)
    }

    companion object {
        private val appointmentHolder = AppointmentDataHolder()
        val instance by lazy { appointmentHolder.getInstance() }
    }
}