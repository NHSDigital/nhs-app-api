package features.authentication.stepDefinitions

import constants.Supplier
import cucumber.api.java.en.Given
import mocking.stubs.appointments.factories.AppointmentsBookingFactory
import mocking.MockingClient
import mocking.defaults.dataPopulation.journies.session.CitizenIdSessionCreateJourney
import mocking.defaults.dataPopulation.journies.session.SessionCreateJourneyFactory
import models.Patient
import utils.SerenityHelpers


class DirectAccessStepDefinitions {

    private val mockingClient = MockingClient.instance

    @Given("^I am about to directly access every page$")
    fun iAmAboutToDirectlyAccessEveryPage() {
        val supplier = Supplier.EMIS
        val patient = Patient.getDefault(supplier)
        SerenityHelpers.setPatient(patient)
        CitizenIdSessionCreateJourney(mockingClient).createFor(patient)
        SessionCreateJourneyFactory.getForSupplier(supplier, mockingClient).createFor(patient)

        val factory = AppointmentsBookingFactory.getForSupplier(supplier)
        factory.generateMultipleAvailableAppointmentSlots()
        factory.generateSuccessfulBookingResponse()
    }
}
