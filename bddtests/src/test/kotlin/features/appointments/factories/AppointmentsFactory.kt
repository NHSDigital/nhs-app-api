package features.appointments.factories

import features.sharedStepDefinitions.GLOBAL_PROVIDER_TYPE
import utils.SerenityHelpers
import mocking.MockingClient
import mocking.defaults.dataPopulation.journies.session.CitizenIdSessionCreateJourney
import mocking.defaults.dataPopulation.journies.session.SessionCreateJourneyFactory
import models.Patient
import net.serenitybdd.core.Serenity

abstract class AppointmentsFactory(gpSupplier: String) {

    val mockingClient = MockingClient.instance
    val patient: Patient = SerenityHelpers.getPatientOrNull() ?: Patient.getDefault(gpSupplier)
    protected val supplier: String = gpSupplier
    protected val appointmentMapper: MockingClientAppointmentMappingFactory

    init {
        SerenityHelpers.setPatient(patient)
        SerenityHelpers.setGpSupplier(supplier)
        Serenity.setSessionVariable(GLOBAL_PROVIDER_TYPE).to(supplier)
        appointmentMapper = MockingClientAppointmentMappingFactory.getForSupplier(supplier)
    }

    fun generateDefaultUserData() {
        CitizenIdSessionCreateJourney(mockingClient).createFor(patient)
        SessionCreateJourneyFactory.getForSupplier(supplier, mockingClient).createFor(patient)
    }

    fun createGetEmptyAppointmentList() {
        val viewAppointmentFactory = UpcomingAppointmentsFactory.getForSupplier(supplier)
        viewAppointmentFactory.createSuccessfulEmptyUpcomingAppointmentResponse()
    }

    companion object {
        const val TargetAppointmentDateKey = "TargetAppointmentDateKey"
        const val TargetAppointmentTimeKey = "TargetAppointmentTimeKey"
    }
}
