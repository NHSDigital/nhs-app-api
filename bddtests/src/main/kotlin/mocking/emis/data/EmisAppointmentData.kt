package mocking.emis.data

import addDays
import addMinutes
import constants.AppointmentDateTimeFormat.Companion.backendDateTimeFormatWithoutTimezone
import constants.AppointmentDateTimeFormat.Companion.frontendDateFormat
import constants.AppointmentDateTimeFormat.Companion.frontendTimeFormat
import mocking.commonData.BaseAppointmentData
import mocking.emis.appointments.GetAppointmentsResponseModel
import mocking.emis.models.Location
import mocking.emis.models.Session
import mocking.emis.models.SessionHolder
import mocking.emis.models.SessionType
import mocking.emis.models.TelephoneAppointmentDetails
import mocking.emis.models.AppointmentCancellationReason
import mocking.emis.models.Appointment
import mockingFacade.appointments.AppointmentSessionFacade
import mockingFacade.appointments.AppointmentSlotFacade
import models.Patient
import models.Slot
import java.text.SimpleDateFormat
import java.util.Calendar
import java.util.TimeZone
import kotlin.collections.ArrayList

private const val LOCATION_ID_SURGERY: Int = 1
private const val LOCATION_ID_HOSPITAL: Int = 2

private const val CLINICIAN_ID_DRSMITH: Int = 1
private const val CLINICIAN_ID_NURSEJONES = 2
private const val CLINICIAN_ID_MSBROWN = 3

private const val SESSION_ID_FOOTCLINIC = 1
private const val SESSION_ID_EYECLINIC = 2
private const val SESSION_ID_EARCLINIC = 3

class EmisAppointmentData private constructor() : BaseAppointmentData() {
    val timeZone = TimeZone.getTimeZone("Europe/London")
    override val dateTimeFormat = createBackendDateTimeFormatWithoutTimezone()
    override val defaultPatient = Patient.getDefault("EMIS")

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

    private fun createBackendDateTimeFormatWithoutTimezone(): SimpleDateFormat {
        val sdf = SimpleDateFormat(backendDateTimeFormatWithoutTimezone)
        sdf.timeZone = timeZone

        return sdf
    }

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

    private val appointments: ArrayList<Appointment> = arrayListOf()

    fun createAppointmentSessions(): ArrayList<AppointmentSessionFacade> {
        val baseTime = Calendar.getInstance(timeZone)

        var startTime = copyCalendarDate(baseTime = baseTime, addDays = 1)
        var sessionDate = dateTimeFormat.format(startTime.time)
        val slot1 = createAppointmentSlot(sessionId = 1, startTime = startTime, durationInMinutes = 15)
        startTime.addDays(1).addMinutes(minutes = 15)
        val slot2 = createAppointmentSlot(sessionId = 2, startTime = startTime, durationInMinutes = 1)
        val footClinicSession = AppointmentSessionFacade(
                sessionId = SESSION_ID_FOOTCLINIC,
                sessionDate = sessionDate,
                slots = arrayListOf(slot1, slot2)
        )

        startTime = copyCalendarDate(baseTime = baseTime, addDays = 2)
        sessionDate = dateTimeFormat.format(startTime.time)
        val slot3 = createAppointmentSlot(sessionId = 3, startTime = startTime, durationInMinutes = 15)
        startTime.addDays(1)
        val slot4 = createAppointmentSlot(sessionId = 4, startTime = startTime, durationInMinutes = 20)
        val eyeClinicSession = AppointmentSessionFacade(
                sessionId = SESSION_ID_EYECLINIC,
                sessionDate = sessionDate,
                slots = arrayListOf(slot3, slot4)
        )

        startTime = copyCalendarDate(baseTime = baseTime, addDays = 4)
        sessionDate = dateTimeFormat.format(startTime.time)
        val slot5 = createAppointmentSlot(sessionId = 3, startTime = startTime, durationInMinutes = 15)
        val earClinicSession = AppointmentSessionFacade(
                sessionId = SESSION_ID_EYECLINIC,
                sessionDate = sessionDate,
                slots = arrayListOf(slot5)
        )
        return arrayListOf(footClinicSession, eyeClinicSession, earClinicSession)
    }

    fun createGetAppointmentsResponse(): GetAppointmentsResponseModel {
        val baseDate = Calendar.getInstance(timeZone)

        var bookingDate = copyCalendarDate(baseDate)
        val appointment1 = addDateToAppointment(unspecifiedTimeAppointment1.copy(), bookingDate, bookInDay = 1, durationInMinutes = 30)
        appointment1.telephoneAppointmentDetails = telephoneAppointmentDetails1

        bookingDate = copyCalendarDate(baseDate, addHours = 2)
        val appointment2 = addDateToAppointment(unspecifiedTimeAppointment2.copy(), bookingDate, bookInDay = 2, durationInMinutes = 20)
        appointment2.telephoneAppointmentDetails = telephoneAppointmentDetails2

        bookingDate = copyCalendarDate(baseDate, addHours = 2, addMinutes = 20)
        val appointment3 = addDateToAppointment(unspecifiedTimeAppointment3.copy(), bookingDate, bookInDay = 4, durationInMinutes = 15)
        appointment3.telephoneAppointmentDetails = telephoneAppointmentDetails1

        appointments.clear()
        appointments.addAll(listOf(appointment1, appointment2, appointment3))

        val appointmentsFromDate = appointment1.startTime

        return GetAppointmentsResponseModel(appointmentsFromDate, appointments, locations, sessionHolders, sessions)
    }

    fun createGetAppointmentsResponseForNoUpcomingAppointments(): GetAppointmentsResponseModel {
        val baseDate = Calendar.getInstance(timeZone)
        val appointmentsFromDate = dateTimeFormat.format(baseDate.time)

        appointments.clear()
        return GetAppointmentsResponseModel(appointmentsFromDate)
    }

    override fun generateExpectedMyAppointments(): ArrayList<Slot> {
        val expectedTempMyAppointments = arrayListOf<Slot>()
        val slotDateFormat = SimpleDateFormat(frontendDateFormat)
        slotDateFormat.timeZone = timeZone
        val slotTimeFormat = SimpleDateFormat(frontendTimeFormat)
        slotTimeFormat.timeZone = timeZone
        appointments.forEach { appointment ->
            val startDate = dateTimeFormat.parse(appointment.startTime)
            val date = slotDateFormat.format(startDate)
            val time = slotTimeFormat.format(startDate).toLowerCase()
            val session = sessions.find { appointment.sessionId == it.sessionId }!!
            val location = locations.find { session.locationId == it.locationId }!!

            val cliniciansNames: ArrayList<String> = ArrayList()
            session.clinicianIds.forEach { clinicianId ->
                cliniciansNames.add(sessionHolders[clinicianId - 1].displayName!!)
            }
            expectedTempMyAppointments.add(expectedMyAppointment.copy(
                    date = date,
                    time = time,
                    session = "${session.sessionName} - ${appointment.slotTypeName}",
                    location = location.locationName,
                    clinician = cliniciansNames
            ))
        }

        return expectedTempMyAppointments
    }


    override fun getAppointmentCancellationReasons(): List<AppointmentCancellationReason> {
        return arrayListOf(emisCancellationReason1, emisCancellationReason2)
    }

    private fun addDateToAppointment(appointment: Appointment, bookingDate: Calendar, bookInDay: Int, durationInMinutes: Int): Appointment {
        appointment.bookingDate = dateTimeFormat.format(bookingDate.time)
        val startDate = bookingDate.addDays(bookInDay)
        appointment.startTime = dateTimeFormat.format(startDate.time)
        appointment.endTime = dateTimeFormat.format(startDate.addMinutes(durationInMinutes).time)
        return appointment
    }

    private fun createAppointmentSlot(sessionId: Int, startTime: Calendar, durationInMinutes: Int): AppointmentSlotFacade {
        val startSession = dateTimeFormat.format(startTime.time)
        val endSession = dateTimeFormat.format(startTime.addMinutes(durationInMinutes).time)
        return AppointmentSlotFacade(sessionId, startSession, endSession)
    }

    private class AppointmentDataHolder {
        private var instance: EmisAppointmentData? = null

        fun getInstance(): EmisAppointmentData {
            if (instance == null) {
                val newInstance = EmisAppointmentData()
                instance = newInstance
            }
            return instance as EmisAppointmentData
        }
    }

    companion object {
        private val appointmentHolder = AppointmentDataHolder()
        val instance by lazy { appointmentHolder.getInstance() }
    }
}
