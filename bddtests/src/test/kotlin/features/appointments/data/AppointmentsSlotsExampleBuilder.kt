package features.appointments.data

import features.appointments.steps.AvailableAppointmentsSteps
import mockingFacade.appointments.AppointmentFilterFacade
import mockingFacade.appointments.AppointmentSessionFacade
import mockingFacade.appointments.AppointmentSlotFacade
import mockingFacade.appointments.AppointmentSlotsResponseFacade
import net.serenitybdd.core.Serenity
import worker.models.appointments.SlotResponseObject
import java.util.*

class AppointmentsSlotsExampleBuilder {

    private var appointmentSessions: ArrayList<AppointmentSessionFacade> = arrayListOf()
    private var filter: AppointmentFilterFacade = AppointmentFilterFacade()
    private var expectedResponseSlots: ArrayList<SlotResponseObject> = arrayListOf()
    private var appointmentTypesList: ArrayList<String> = arrayListOf()
    private var locationsList: ArrayList<String> = arrayListOf()
    private var cliniciansList: ArrayList<String> = arrayListOf()

    fun appointmentSessions(value: ArrayList<AppointmentSessionFacade>): AppointmentsSlotsExampleBuilder {
        appointmentSessions = value
        return this
    }

    fun filterValues(value: AppointmentFilterFacade): AppointmentsSlotsExampleBuilder {
        filter = value
        return this
    }

    fun expectedResponseSlots(value: ArrayList<SlotResponseObject>): AppointmentsSlotsExampleBuilder {
        expectedResponseSlots = value
        return this
    }

    fun appointmentTypesList(value: ArrayList<String>): AppointmentsSlotsExampleBuilder {
        appointmentTypesList = value
        return this
    }

    fun locationsList(value: ArrayList<String>): AppointmentsSlotsExampleBuilder {
        locationsList = value
        return this
    }

    fun cliniciansList(value: ArrayList<String>): AppointmentsSlotsExampleBuilder {
        cliniciansList = value
        return this
    }

    fun build(): AppointmentSlotsResponseFacade {
        return getExample()
    }

    private fun getExample(): AppointmentSlotsResponseFacade {

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
}