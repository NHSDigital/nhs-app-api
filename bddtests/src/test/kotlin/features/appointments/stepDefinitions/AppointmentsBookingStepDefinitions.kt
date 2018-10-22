package features.appointments.stepDefinitions

import cucumber.api.java.en.Given
import features.appointments.factories.AppointmentsBookingFactory
import features.appointments.factories.AppointmentsBookingFactory.Companion.SymptomsToEnter
import mocking.emis.practices.NecessityOption
import net.serenitybdd.core.Serenity
import java.time.Duration

class AppointmentsBookingStepDefinitions {

    @Given("^there are (.*) appointments available to book$")
    fun thereAreAvailableAppointmentsToBook(gpSystem: String) {
        val factory = AppointmentsBookingFactory.getForSupplier(gpSystem)
        factory.generateDefaultAvailableAppointmentSlotExample()
        factory.generateSuccessfulBookingResponse()
    }

    @Given("^there are multiple appointment slots at the same time, provided by (.*)$")
    fun thereAreAvailableAppointmentsAtTheSameTime(gpSystem: String) {
        val factory = AppointmentsBookingFactory.getForSupplier(gpSystem)
        factory.generateMultipleAvailableAppointmentSlots()
        factory.generateSuccessfulBookingResponse()
    }

    @Given("^there are (.*) appointments available to book with a reason of (\\d+) character$")
    fun thereAreAvailableAppointmentsToBookWithAReasonOfNumberOfCharactersSpecified(gpSystem: String, numberOfCharacters: Int) {
        val bookingReasonOfSpecifiedLength = "x".repeat(numberOfCharacters)
        val factory = AppointmentsBookingFactory.getForSupplier(gpSystem)
        factory.generateDefaultAvailableAppointmentSlotExample()
        factory.generateSuccessfulBookingResponse(bookingReasonOfSpecifiedLength)
    }

    @Given("^there are (.*) appointments available to book with a reason of (\\d+) characters but user attempts to enter (\\d+) characters$")
    fun thereAreAvailableAppointmentsToBookWithAReasonOfNumberOfCharactersSpecified(gpSystem: String, numberOfCharactersAllowed: Int, numberOfCharactersEntered: Int) {
        val bookingReasonOfSpecifiedLength = "x".repeat(numberOfCharactersAllowed)
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

    //    EMIS Specific step definition
    @Given("^there are EMIS appointments available to book where booking reason is set optional$")
    fun thereAreEMISAppointmentsAvailableToBookWhereBookingReasonIsSetOptional() {
        val factory = AppointmentsBookingFactory.getForSupplier("EMIS")
        factory.generateDefaultAvailableAppointmentSlotExample(reasonNecessityOption = NecessityOption.OPTIONAL)
        factory.generateSuccessfulBookingResponseEmptyReasong()
    }

    @Given("^there are EMIS appointments available to book where booking reason is set optional with 150 reason characters entered$")
    fun thereAreEMISAppointmentsAvailableToBookWhereBookingReasonIsSetOptionalWith150ReasonCharactersEntered() {
        val factory = AppointmentsBookingFactory.getForSupplier("EMIS")
        factory.generateDefaultAvailableAppointmentSlotExample(reasonNecessityOption = NecessityOption.OPTIONAL)
        val symptomsToEnter = "x".repeat(150)
        Serenity.setSessionVariable(SymptomsToEnter).to(symptomsToEnter)
        factory.generateSuccessfulBookingResponse(symptomsToEnter)
    }

    @Given("^there are EMIS appointments available to book where booking reason option is set not required$")
    fun thereAreEMISAppointmentsAvailableToBookWhereBookingReasonIsSetRequired() {
        val factory = AppointmentsBookingFactory.getForSupplier("EMIS")
        factory.generateDefaultAvailableAppointmentSlotExample(reasonNecessityOption = NecessityOption.NOT_ALLOWED)
        factory.generateSuccessfulBookingResponseEmptyReasong()
    }
}
