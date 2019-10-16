package mocking.tpp.data

import mocking.stubs.appointments.CANCEL_APPOINTMENT_SLOT_ID
import mockingFacade.appointments.AppointmentSessionFacade
import mockingFacade.appointments.AppointmentSlotFacade
import mockingFacade.appointments.AppointmentSlotsResponseFacade
import mockingFacade.appointments.MyAppointmentsFacade
import mockingFacade.appointments.metadata.LocationFacade
import mockingFacade.appointments.metadata.SlotTypeFacade
import mockingFacade.appointments.metadata.StaffDetailsFacade
import java.time.ZonedDateTime

private const val APPOINTMENT_OFFSET_AMOUNT = 30L

class TppAppointmentData private constructor() {
    private val clinicianIdDrSmith = StaffDetailsFacade(1, "Dr Smith")
    private val clinicianIdNurseJones = StaffDetailsFacade(2, "Nurse Jones")
    private val locationEyeClinic = LocationFacade(1, "Eye Clinic")
    private val locationEarClinic = LocationFacade(2, "Ear Clinic")

    fun createGetAppointmentsResponse(): MyAppointmentsFacade {
        val futureDate = ZonedDateTime.now().plusDays(APPOINTMENT_OFFSET_AMOUNT)
        val pastDate = ZonedDateTime.now().minusDays(APPOINTMENT_OFFSET_AMOUNT)
        val pastAppointmentSlotId = 2
        val sessions = listOf(

                AppointmentSessionFacade(locationId = locationEarClinic.locationId, slots= listOf(
                        AppointmentSlotFacade(  slotId = CANCEL_APPOINTMENT_SLOT_ID,
                                                startTime = futureDate.toString(),
                                                endTime = futureDate.plusMinutes(APPOINTMENT_OFFSET_AMOUNT).toString(),
                                                slotDetails = "Details about this appointment"))),

                AppointmentSessionFacade(locationId = locationEyeClinic.locationId, slots = listOf(
                        AppointmentSlotFacade(  slotId = pastAppointmentSlotId,
                                                startTime = pastDate.toString(),
                                                endTime = pastDate.plusMinutes(APPOINTMENT_OFFSET_AMOUNT).toString(),
                                                slotDetails = "PAST APPOINTMENT DETAILS"))))

        val locations = listOf(locationEyeClinic, locationEarClinic)
        val staffDetails = listOf(clinicianIdDrSmith, clinicianIdNurseJones)
        val slotType = listOf(SlotTypeFacade(1 ,"Timed"))

        val slotsResponseFacade = AppointmentSlotsResponseFacade(sessions = sessions, locations = locations,
                staffDetails = staffDetails, slotTypes = slotType)

        return convertToMyAppointmentsFacade(slotsResponseFacade)
    }

    private fun convertToMyAppointmentsFacade(facade: AppointmentSlotsResponseFacade): MyAppointmentsFacade {
        return MyAppointmentsFacade(facade)
    }

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

