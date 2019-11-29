package mocking.emis.data

import addDays
import addMinutes
import constants.DateTimeFormats.Companion.backendDateTimeFormatWithoutTimezone
import constants.Supplier
import mocking.commonData.BaseAppointmentData
import mocking.emis.appointments.GetAppointmentsResponseModel
import mocking.emis.models.Appointment
import mocking.emis.models.AppointmentCancellationReason
import mocking.emis.models.Location
import mocking.emis.models.Session
import mocking.emis.models.SessionHolder
import mocking.emis.models.SessionType
import mocking.emis.models.TelephoneAppointmentDetails
import models.Patient
import models.Slot
import java.text.SimpleDateFormat
import java.util.*

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
    override val defaultPatient = Patient.getDefault(Supplier.EMIS)

    private val expectedMyAppointment = Slot(slotType = SessionType.Timed.toString())

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

    fun createGetAppointmentsResponse(): GetAppointmentsResponseModel {
        val baseDate = Calendar.getInstance(timeZone)

        var bookingDate = copyCalendarDate(baseDate)
        val appointment1 = addDateToAppointment(unspecifiedTimeAppointment1.copy(),
                                                bookingDate, bookInDay = 1, durationInMinutes = 30)
        appointment1.telephoneAppointmentDetails = telephoneAppointmentDetails1

        bookingDate = copyCalendarDate(baseDate, addHours = 2)
        val appointment2 = addDateToAppointment(unspecifiedTimeAppointment2.copy(),
                                                bookingDate, bookInDay = 2, durationInMinutes = 20)
        appointment2.telephoneAppointmentDetails = telephoneAppointmentDetails2

        bookingDate = copyCalendarDate(baseDate, addHours = 2, addMinutes = 20)
        val appointment3 = addDateToAppointment(unspecifiedTimeAppointment3.copy(),
                                                bookingDate, bookInDay = 4, durationInMinutes = 15)
        appointment3.telephoneAppointmentDetails = telephoneAppointmentDetails1

        appointments.clear()
        appointments.addAll(listOf(appointment1, appointment2, appointment3))

        val appointmentsFromDate = appointment1.startTime

        return GetAppointmentsResponseModel(appointmentsFromDate, appointments,
                                            locations, sessionHolders, sessions)
    }

    override fun getAppointmentCancellationReasons(): List<AppointmentCancellationReason> {
        return arrayListOf(emisCancellationReason1, emisCancellationReason2)
    }

    private fun addDateToAppointment(appointment: Appointment, bookingDate: Calendar,
                                     bookInDay: Int, durationInMinutes: Int): Appointment {
        appointment.bookingDate = dateTimeFormat.format(bookingDate.time)
        val startDate = bookingDate.addDays(bookInDay)
        appointment.startTime = dateTimeFormat.format(startDate.time)
        appointment.endTime = dateTimeFormat.format(startDate.addMinutes(durationInMinutes).time)
        return appointment
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
