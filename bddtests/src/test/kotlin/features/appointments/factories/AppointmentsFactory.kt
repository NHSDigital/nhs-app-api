package features.appointments.factories

import utils.SerenityHelpers
import mocking.MockingClient
import mocking.defaults.dataPopulation.journies.session.CitizenIdSessionCreateJourney
import mocking.defaults.dataPopulation.journies.session.SessionCreateJourneyFactory
import models.Patient

abstract class AppointmentsFactory(gpSupplier: String) {

    val mockingClient = MockingClient.instance
    val patient: Patient = SerenityHelpers.getPatientOrNull() ?: Patient.getDefault(gpSupplier)
    protected val supplier: String = gpSupplier
    protected val appointmentMapper: MockingClientAppointmentMappingFactory

    init {
        SerenityHelpers.setPatient(patient)
        appointmentMapper = MockingClientAppointmentMappingFactory.getForSupplier(supplier)
    }

    fun generateDefaultUserData() {
        CitizenIdSessionCreateJourney(mockingClient).createFor(patient)
        SessionCreateJourneyFactory.getForSupplier(supplier, mockingClient).createFor(patient)
        createGetEmptyAppointmentList()
    }

    private fun createGetEmptyAppointmentList() {
        val viewAppointmentFactory = UpcomingAppointmentsFactory.getForSupplier(supplier)
        viewAppointmentFactory.createSuccessfulEmptyUpcomingAppointmentResponse()
    }

    companion object {
        const val TargetAppointmentDateKey = "TargetAppointmentDateKey"
        const val TargetAppointmentTimeKey = "TargetAppointmentTimeKey"
    }
}