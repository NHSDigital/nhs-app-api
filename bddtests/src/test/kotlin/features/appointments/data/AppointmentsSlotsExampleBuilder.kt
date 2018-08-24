package features.appointments.data

import features.appointments.steps.AvailableAppointmentsSteps
import mockingFacade.appointments.AppointmentFilterFacade
import mockingFacade.appointments.AppointmentSessionFacade
import mockingFacade.appointments.AppointmentSlotFacade
import mockingFacade.appointments.AppointmentSlotsResponseFacade
import net.serenitybdd.core.Serenity
import net.serenitybdd.core.Serenity.setSessionVariable
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
        setSessionVariable(AvailableAppointmentsSteps.AppointmentSessionVariableKeys.EXPECTED_APPOINTMENT_SESSIONS_KEY).to(appointmentSessions)

        setSessionVariable(AppointmentSlotExpectations.EXPECTED_APPOINTMENT_FILTER_FACADE_KEY).to(filter)
        setSessionVariable(AppointmentSlotExpectations.EXPECTED_APPOINTMENT_TYPE_KEY).to(appointmentTypesList)
        setSessionVariable(AppointmentSlotExpectations.EXPECTED_APPOINTMENT_LOCATIONS_KEY).to(locationsList)
        setSessionVariable(AppointmentSlotExpectations.EXPECTED_APPOINTMENT_CLINICIANS_KEY).to(cliniciansList)
        setSessionVariable(AppointmentSlotExpectations.EXPECTED_RESPONSE_SLOTS_KEY).to(expectedResponseSlots)
    }

    enum class AppointmentSlotExpectations{
        EXPECTED_APPOINTMENT_TYPE_KEY,
        EXPECTED_APPOINTMENT_LOCATIONS_KEY,
        EXPECTED_APPOINTMENT_CLINICIANS_KEY,
        EXPECTED_APPOINTMENT_FILTER_FACADE_KEY,
        EXPECTED_RESPONSE_SLOTS_KEY
    }
}