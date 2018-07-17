package features.appointments.stepDefinitions.factories

import features.appointments.steps.AvailableAppointmentsSteps
import mockingFacade.appointments.AppointmentSlotsResponseFacade
import models.Patient
import net.serenitybdd.core.Serenity

class AppointmentsBookingFactoryTpp: AppointmentsBookingFactory("TPP"){

    override fun generate() {
        Serenity.setSessionVariable(AvailableAppointmentsSteps.EXPECTED_APPOINTMENT_SESSIONS_KEY)
                .to(defaultAppointmentSessions)

        val listSlotsReply = AppointmentSlotsResponseFacade(defaultAppointmentSessions, "1")
        var patient = Serenity.sessionVariableCalled<Patient>(Patient::class)

        generateAppointmentSlotResponse(patient, listSlotsReply, 0)
    }
}