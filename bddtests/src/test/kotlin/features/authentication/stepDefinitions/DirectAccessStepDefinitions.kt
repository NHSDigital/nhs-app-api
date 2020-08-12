package features.authentication.stepDefinitions

import constants.Supplier
import io.cucumber.java.en.Given
import mocking.defaults.dataPopulation.journies.session.CitizenIdSessionCreateJourney
import mocking.defaults.dataPopulation.journies.session.SessionCreateJourneyFactory
import mocking.stubs.appointments.factories.AppointmentsBookingFactory
import models.Patient
import utils.SerenityHelpers


class DirectAccessStepDefinitions {

    @Given("^I am about to directly access every page$")
    fun iAmAboutToDirectlyAccessEveryPage() {
        val supplier = Supplier.EMIS
        val patient = Patient.getDefault(supplier)
        SerenityHelpers.setPatient(patient)
        CitizenIdSessionCreateJourney().createFor(patient)
        SessionCreateJourneyFactory.getForSupplier(supplier).createFor(patient)

        val factory = AppointmentsBookingFactory.getForSupplier(supplier)
        factory.generateMultipleAvailableAppointmentSlots()
        factory.generateSuccessfulBookingResponse()
    }
}
