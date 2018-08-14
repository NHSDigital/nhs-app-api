package features.appointments.stepDefinitions

import cucumber.api.java.en.Given
import features.appointments.factories.AppointmentsBookingFactory
import features.appointments.factories.AppointmentsBookingFactory.Companion.SymptomsToEnter
import net.serenitybdd.core.Serenity
import java.time.Duration


class AppointmentsBookingStepDefinitions {

    @Given("^there are (.*) appointments available to book$")
    fun thereAreAvailableAppointmentsToBook(gpSystem: String) {
        val factory = AppointmentsBookingFactory.getForSupplier(gpSystem)
        factory.generateDefaultAvailableAppointmentSlotExample()
        factory.generateSuccessfulBookingResponse()
    }

    @Given("^there are (.*) appointments available to book with a reason of (\\d+) character$")
    fun thereAreAvailableAppointmentsToBookWithAReasonOfNumberOfCharactersSpecified(gpSystem: String, numberOfCharacters: Int) {
        var bookingReasonOfSpecifiedLength = "x".repeat(numberOfCharacters)
        val factory = AppointmentsBookingFactory.getForSupplier(gpSystem)
        factory.generateDefaultAvailableAppointmentSlotExample()
        factory.generateSuccessfulBookingResponse(bookingReasonOfSpecifiedLength)
    }

    @Given("^there are (.*) appointments available to book with a reason of (\\d+) characters but user attempts to enter (\\d+) characters$")
    fun thereAreAvailableAppointmentsToBookWithAReasonOfNumberOfCharactersSpecified(gpSystem: String,numberOfCharactersAllowed: Int, numberOfCharactersEntered: Int) {
        var bookingReasonOfSpecifiedLength = "x".repeat(numberOfCharactersAllowed)
        val symptomsToEnter = "x".repeat(numberOfCharactersEntered)
        val factory = AppointmentsBookingFactory.getForSupplier(gpSystem)
        factory.generateDefaultAvailableAppointmentSlotExample()
        factory.generateSuccessfulBookingResponse(bookingReasonOfSpecifiedLength)
        Serenity.setSessionVariable(SymptomsToEnter).to(symptomsToEnter)
    }

    @Given("^there are (.*) appointments available to book, but GP system doesn't respond a timely fashion when booking$")
    fun thereAreAvailableAppointmentsToBookButSystemDoesNotRespond(gpSystem: String) {
        val factory = AppointmentsBookingFactory.getForSupplier(gpSystem)
        factory.generateDefaultAvailableAppointmentSlotExample()
        factory.generateBookingResponse { bookRequest -> bookRequest.withDelay(Duration.ofSeconds(12)).respondWithSuccess() }
    }

    @Given("^there are (.*) appointments available to book, but the GP system is unavailable$")
    fun thereAreAvailableAppointmentsToBookButSystemIsUnavailable(gpSystem: String) {

        val factory = AppointmentsBookingFactory.getForSupplier(gpSystem)
        factory.generateDefaultAvailableAppointmentSlotExample()
        factory.generateBookingResponse { bookRequest -> bookRequest.respondWithUnavailableException() }
    }

    @Given("^there are (.*) appointments available to book, but the appointment slot has already been booked by somebody else$")
    fun thereAreAvailableAppointmentsToBookButBookingConflict(gpSystem: String) {

        val factory = AppointmentsBookingFactory.getForSupplier(gpSystem)
        factory.generateDefaultAvailableAppointmentSlotExample()
        factory.generateBookingResponse { bookRequest -> bookRequest.respondWithConflictException() }
    }
}
