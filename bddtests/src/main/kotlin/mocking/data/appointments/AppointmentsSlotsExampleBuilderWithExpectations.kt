package mocking.data.appointments

import mockingFacade.appointments.AppointmentSlotFacade
import mockingFacade.appointments.AppointmentSlotsResponseFacade
import net.serenitybdd.core.Serenity.setSessionVariable

class AppointmentsSlotsExampleBuilderWithExpectations : AppointmentsSlotsExampleBuilder() {

    override fun build(): AppointmentSlotsResponseFacade {
        return getExample()
    }

    private fun getExample(): AppointmentSlotsResponseFacade {
        setSessionVariable(AppointmentSlotSerenityKeys.APPOINTMENT_SLOTS_EXAMPLE_SESSIONS)
                .to(appointmentSessions)
        setExpectations()
        return AppointmentSlotsResponseFacade(
                appointmentSessions,
                "1",
                locationsList,
                cliniciansList,
                appointmentTypesList
        )
    }

    private fun setExpectations() {
        val appointmentSlots = arrayListOf<AppointmentSlotFacade>()
        appointmentSlots.addAll(appointmentSessions.flatMap { session -> session.slots })
        setSessionVariable(AppointmentSessionVariableKeys.EXPECTED_APPOINTMENT_SESSIONS_KEY)
                .to(appointmentSessions)

        setSessionVariable(AppointmentSlotSerenityKeys.APPOINTMENT_FILTER_FACADE_KEY).to(filter)
        setSessionVariable(AppointmentSlotSerenityKeys.EXPECTED_APPOINTMENT_TYPE_KEY)
                .to(appointmentTypesList)
        setSessionVariable(AppointmentSlotSerenityKeys.EXPECTED_APPOINTMENT_LOCATIONS_KEY)
                .to(locationsList)
        setSessionVariable(AppointmentSlotSerenityKeys.EXPECTED_APPOINTMENT_CLINICIANS_KEY)
                .to(cliniciansList)
    }

    enum class AppointmentSlotSerenityKeys {
        APPOINTMENT_SLOTS_EXAMPLE_SESSIONS,
        EXPECTED_APPOINTMENT_TYPE_KEY,
        EXPECTED_APPOINTMENT_LOCATIONS_KEY,
        EXPECTED_APPOINTMENT_CLINICIANS_KEY,
        APPOINTMENT_FILTER_FACADE_KEY
    }
}