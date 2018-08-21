package features.appointments.data

import constants.AppointmentDateTimeFormat
import features.appointments.factories.IdValue
import features.appointments.steps.AvailableAppointmentsSteps
import mockingFacade.appointments.AppointmentFilterFacade
import mockingFacade.appointments.AppointmentSessionFacade
import mockingFacade.appointments.AppointmentSlotFacade
import mockingFacade.appointments.AppointmentSlotsResponseFacade
import net.serenitybdd.core.Serenity
import worker.models.appointments.SlotResponseObject
import java.time.LocalDateTime
import java.time.format.DateTimeFormatter
import java.util.ArrayList

abstract class AppointmentsSlotsExampleBase {

    protected var dateFormatter = DateTimeFormatter.ofPattern(AppointmentDateTimeFormat.backendDateTimeFormatWithoutTimezone)
    protected var tomorrowDate = LocalDateTime.now().plusDays(1)

    protected abstract val appointmentSessions: ArrayList<AppointmentSessionFacade>
    protected abstract val filter: AppointmentFilterFacade
    protected abstract val expectedResponseSlots: ArrayList<SlotResponseObject>
    protected abstract val appointmentTypesList: ArrayList<String>
    protected abstract val locationsList: ArrayList<String>
    protected abstract val cliniciansList: ArrayList<String>


    fun getExample(): AppointmentSlotsResponseFacade {

        val getAppointmentSlotsResponseModel = AppointmentSlotsResponseFacade(appointmentSessions, "1")
        setExpectations(appointmentSessions)
        return getAppointmentSlotsResponseModel
    }


    private fun setExpectations(appointmentSessions: ArrayList<AppointmentSessionFacade>) {
        val appointmentSlots = arrayListOf<AppointmentSlotFacade>()
        appointmentSlots.addAll(appointmentSessions.flatMap { session -> session.slots })
        Serenity.setSessionVariable(AvailableAppointmentsSteps.AppointmentSessionVariableKeys.EXPECTED_APPOINTMENT_SESSIONS_KEY).to(appointmentSessions)

        Serenity.setSessionVariable(EXPECTED_APPOINTMENT_FILTER_FACADE_KEY).to(filter)
        Serenity.setSessionVariable(EXPECTED_APPOINTMENT_TYPE_KEY).to(appointmentTypesList)
        Serenity.setSessionVariable(EXPECTED_APPOINTMENT_LOCATIONS_KEY).to(locationsList)
        Serenity.setSessionVariable(EXPECTED_APPOINTMENT_CLINICIANS_KEY).to(cliniciansList)
        Serenity.setSessionVariable(EXPECTED_RESPONSE_SLOTS_KEY).to(expectedResponseSlots)
    }

    companion object {


        const val EXPECTED_APPOINTMENT_TYPE_KEY = "ExpectedAppointmentTypesKey"
        const val EXPECTED_APPOINTMENT_LOCATIONS_KEY = "ExpectedAppointmentLocationsKey"
        const val EXPECTED_APPOINTMENT_CLINICIANS_KEY = "ExpectedAppointmentCliniciansKey"
        const val EXPECTED_APPOINTMENT_FILTER_FACADE_KEY = "ExpectedAppointmentFilterFacadeKey"
        const val EXPECTED_RESPONSE_SLOTS_KEY = "ExpectedResponseSlotsKey"
    }

    protected val clinic = "Clinic"
    protected val clinicSlot = "Clinic - Slot"
    protected val locationLeeds = IdValue (1, "Leeds")
    protected val locationSheffield = IdValue (2, "Sheffield")
    protected val staffDrWho = IdValue (101, "Dr. Who")
    protected val staffDrScott = IdValue (102, "Dr. Scott")
}