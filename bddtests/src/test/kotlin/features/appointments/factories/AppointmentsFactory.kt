package features.appointments.factories

import features.sharedSteps.SerenityHelpers
import mocking.MockingClient
import mocking.defaults.dataPopulation.journies.session.CitizenIdSessionCreateJourney
import mocking.defaults.dataPopulation.journies.session.SessionCreateJourneyFactory
import models.Patient

abstract class AppointmentsFactory(gpSupplier: String) {

    val mockingClient = MockingClient.instance
    var patient: Patient
    protected var supplier: String = gpSupplier
    protected var appointmentMapper: MockingClientAppointmentMappingFactory

    init {
        patient = SerenityHelpers.getPatientOrNull()?: Patient.getDefault(gpSupplier)
        SerenityHelpers.setPatient(patient)
        appointmentMapper = MockingClientAppointmentMappingFactory.getForSupplier(supplier)
    }

    fun generateDefaultUserData() {
        CitizenIdSessionCreateJourney(mockingClient).createFor(patient)
        SessionCreateJourneyFactory.getForSupplier(supplier, mockingClient).createFor(patient)
        createGetEmptyAppointmentList()
    }

    private fun createGetEmptyAppointmentList() {
        val viewAppointmentFactory = ViewAppointmentsFactory.getForSupplier(supplier)
        val getResponse = viewAppointmentFactory.createEmptyUpcomingAppointmentResponse(patient)
        appointmentMapper
                .requestMapping { viewMyAppointmentsRequest(patient).respondWithSuccess(getResponse) }
    }

    companion object {
        const val AppointmentStartTimeKey = "AppointmentStartTimeKey"
        const val AppointmentEndTimeKey = "AppointmentEndTimeKey"

        const val TargetAppointmentDateKey = "TargetAppointmentDateKey"
        const val TargetAppointmentTimeKey = "TargetAppointmentTimeKey"
    }
}