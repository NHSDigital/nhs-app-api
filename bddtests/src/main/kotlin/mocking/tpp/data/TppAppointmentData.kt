package mocking.tpp.data

import addDays
import addHours
import addMinutes
import constants.AppointmentDateTimeFormat
import mocking.commonData.BaseAppointmentData
import mocking.emis.models.AppointmentCancellationReason
import mocking.emis.models.SessionType
import mocking.tpp.models.Appointment
import mocking.tpp.models.Session
import mocking.tpp.models.ViewAppointmentsReply
import models.Patient
import models.Slot
import java.text.SimpleDateFormat
import java.util.*
import kotlin.collections.ArrayList

class TppAppointmentData private constructor() : BaseAppointmentData() {

    override val dateTimeFormat = SimpleDateFormat(AppointmentDateTimeFormat.tppDateTimeFormat)
    override val defaultPatient = Patient.getDefault("TPP")

    private val sessionTypes = arrayOf("Session A", "Session B")
    private val staffs = arrayOf("Dr James Jones (Male)", "Sean Devlin")
    private val clinicianRole = "Clinician"
    private val appointmentDetailList = arrayOf("$clinicianRole: Dr James Jones", "$clinicianRole: Sean Devlin")
    private val appointmentSiteName = "Kainos GP Demo Unit"
    private val locations = arrayOf("Leeds", "Sheffield")
    private val addresses = arrayListOf("Tpp, Leeds, West Yorkshire, LS18 5PX", "Tpp, Sheffield, South Yorkshire, S15 5PX")

    private val appointments = arrayListOf<Appointment>()

    val drJamesBaseSession = Session(
            type = sessionTypes[0],
            staffDetails = staffs[0],
            location = locations[0])

    val seanBaseSession = Session(
            type = sessionTypes[1],
            staffDetails = staffs[1],
            location = locations[1])

    val drJamesBaseTppAppointment = Appointment(
            details = appointmentDetailList[0],
            siteName = appointmentSiteName,
            address = addresses[0],
            apptId = TppConfig.appId)

    val seanBaseTppAppointment = drJamesBaseTppAppointment.copy(details = appointmentDetailList[1])

    fun createTppAppointmentsResponse(patient: Patient): ViewAppointmentsReply {
        val baseTime = Calendar.getInstance()
        val appointmentsReply = createEmptyTppMyAppointmentResponse(patient)

        appointments.clear()
        appointments.addAll(generateTppAppointmentFor(drJamesBaseTppAppointment, baseTime))
        appointments.addAll(generateTppAppointmentFor(seanBaseTppAppointment, baseTime.addDays(2)))
        appointments.sortBy { dateTimeFormat.parse(it.startDate).time }

        appointmentsReply.Appointment = appointments
        return appointmentsReply
    }

    fun createEmptyTppMyAppointmentResponse(patient: Patient): ViewAppointmentsReply {
        appointments.clear()
        return ViewAppointmentsReply(
                patientId = patient.patientId,
                onlineUserId = patient.onlineUserId,
                uuid = TppConfig.uuid)
    }

    private fun generateTppAppointmentFor(baseTppAppointment: Appointment, withBaseDate: Calendar, howMany: Int = 4): ArrayList<Appointment> {
        val duration = 15
        val appointmentTime = copyCalendarDate(withBaseDate)
        val appointments = arrayListOf<Appointment>()
        for (counter in 1..howMany) {
            if (counter % 2 == 0)
                appointmentTime.addDays(1).set(Calendar.HOUR_OF_DAY, 9)
            val startTime = dateTimeFormat.format(appointmentTime.time)
            val endTime = dateTimeFormat.format(appointmentTime.addMinutes(duration).time)
            appointments.add(baseTppAppointment.copy(startDate = startTime, endDate = endTime))
        }
        return appointments
    }

    override fun generateExpectedMyAppointments(timezone: String): ArrayList<Slot> {
        val webSlots = arrayListOf<Slot>()
        val webSlot = Slot(session = SessionType.Timed.toString())
        val slotDateFormat = SimpleDateFormat(AppointmentDateTimeFormat.frontendDateFormat)
        val slotTimeFormat = SimpleDateFormat(AppointmentDateTimeFormat.frontendTimeFormat)
        val dateFormatWithTimeZone = SimpleDateFormat(AppointmentDateTimeFormat.backendDateTimeFormatWithTimezone)
        appointments.forEach { appointment ->
            val frontendTime = convertToBrowserTimezone(appointment.startDate, timezone)
            val startDate = dateFormatWithTimeZone.parse(frontendTime)
            val date = slotDateFormat.format(startDate)
            val time = slotTimeFormat.format(startDate).toLowerCase()
            val location = appointment.siteName

            webSlots.add(webSlot.copy(
                    date = date,
                    time = time,
                    session = appointment.details,
                    location = location
            ))
        }
        return webSlots
    }

    override fun getAppointmentCancellationReasons(): List<AppointmentCancellationReason>? = null

    private class AppointmentDataHolder {
        private var instance: TppAppointmentData? = null

        fun getInstance(): TppAppointmentData {
            if (instance == null) {
                val newInstance = TppAppointmentData()
                instance = newInstance
            }
            return instance as TppAppointmentData
        }
    }

    companion object {
        private val appointmentHolder = AppointmentDataHolder()
        val instance by lazy { appointmentHolder.getInstance() }
    }
}