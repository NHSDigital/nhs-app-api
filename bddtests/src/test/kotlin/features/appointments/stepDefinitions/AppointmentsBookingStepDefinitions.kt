package features.appointments.stepDefinitions

import cucumber.api.java.en.Given
import features.appointments.factories.AppointmentsBookingFactory
import features.appointments.factories.AppointmentsBookingFactory.Companion.symptomsToEnter
import features.appointments.factories.UpcomingAppointmentsFactory
import features.appointments.steps.AppointmentsConfirmationSteps
import features.sharedSteps.SupplierSpecificFactory
import mocking.emis.appointments.BookAppointmentsBuilderEmis
import mocking.emis.practices.NecessityOption
import net.serenitybdd.core.Serenity
import utils.SerenityHelpers
import java.time.Duration

private const val DELAY_IN_SECONDS = 12L

class AppointmentsBookingStepDefinitions {

    private var emptyReason: Boolean = false

    @Given("^there are (.*) appointments available to book$")
    fun thereAreAvailableAppointmentsToBook(gpSystem: String) {
        val factory = AppointmentsBookingFactory.getForSupplier(gpSystem)
        factory.generateDefaultAvailableAppointmentSlotExample()
        factory.generateSuccessfulBookingResponse()
    }

    @Given("^a booked appointment can be cancelled$")
    fun aBookedAppointmentCanBeCancelled() {
        val gpSystem = Serenity.sessionVariableCalled<String>(SupplierSpecificFactory.SerenityKey.GP_SYSTEM)
        val viewAppointmentFactory = UpcomingAppointmentsFactory.getForSupplier(gpSystem)
        viewAppointmentFactory.createSuccessfulUpcomingAppointmentsResponseOnceBooked()
    }

    @Given("^there are multiple appointment slots at the same time, provided by (.*)$")
    fun thereAreAvailableAppointmentsAtTheSameTime(gpSystem: String) {
        val factory = AppointmentsBookingFactory.getForSupplier(gpSystem)
        factory.generateMultipleAvailableAppointmentSlots()
        factory.generateSuccessfulBookingResponse()
    }

    @Given("^there are (.*) appointments available to book with a reason of (\\d+) character$")
    fun thereAreAvailableAppointmentsToBookWithAReasonOfNumberOfCharactersSpecified(gpSystem: String,
                                                                                    numberOfCharacters: Int) {
        val bookingReasonOfSpecifiedLength = "x".repeat(numberOfCharacters)
        val factory = AppointmentsBookingFactory.getForSupplier(gpSystem)
        factory.generateDefaultAvailableAppointmentSlotExample()
        factory.generateSuccessfulBookingResponse(bookingReasonOfSpecifiedLength)
    }

    @Given("^there are (.*) appointments available to book with a reason of (\\d+) characters " +
            "but user attempts to enter (\\d+) characters$")
    fun thereAreAvailableAppointmentsToBookWithAReasonOfNumberOfCharactersSpecified(gpSystem: String,
                                                                                    numberOfCharactersAllowed: Int,
                                                                                    numberOfCharactersEntered: Int) {
        val bookingReasonOfSpecifiedLength = "x".repeat(numberOfCharactersAllowed)
        val symptomsToEnter = "x".repeat(numberOfCharactersEntered)
        val factory = AppointmentsBookingFactory.getForSupplier(gpSystem)
        factory.generateDefaultAvailableAppointmentSlotExample()
        factory.generateSuccessfulBookingResponse(bookingReasonOfSpecifiedLength)
        Serenity.setSessionVariable(AppointmentsBookingFactory.symptomsToEnter).to(symptomsToEnter)
    }

    @Given("^there are (.*) appointments available to book and user attempts to enter booking reason (.*)\$")
    fun thereAreAvailableAppointmentsToBookWithABookingReason(gpSystem: String, bookingReason: String) {
        val factory = AppointmentsBookingFactory.getForSupplier(gpSystem)
        factory.generateDefaultAvailableAppointmentSlotExample()
        factory.generateSuccessfulBookingResponse(bookingReason)
        Serenity.setSessionVariable(symptomsToEnter).to(bookingReason)
    }

    @Given("^there are (.*) appointments available to book, " +
            "but GP system doesn't respond a timely fashion when booking$")
    fun thereAreAvailableAppointmentsToBookButSystemDoesNotRespond(gpSystem: String) {
        val factory = AppointmentsBookingFactory.getForSupplier(gpSystem)
        factory.generateDefaultAvailableAppointmentSlotExample()

        factory.generateBookingResponse { bookRequest ->
            bookRequest.withDelay(Duration.ofSeconds(DELAY_IN_SECONDS))
                    .respondWithSuccess()
        }
    }

    @Given("^there are (.*) appointments available to book, but the GP system is unavailable$")
    fun thereAreAvailableAppointmentsToBookButSystemIsUnavailable(gpSystem: String) {

        val factory = AppointmentsBookingFactory.getForSupplier(gpSystem)
        factory.generateDefaultAvailableAppointmentSlotExample()
        factory.generateBookingResponse { bookRequest -> bookRequest.respondWithGPServiceUnavailableException() }
    }

    @Given("^there are (.*) appointments available to book, " +
            "but the appointment slot has already been booked by somebody else$")
    fun thereAreAvailableAppointmentsToBookButBookingConflict(gpSystem: String) {

        val factory = AppointmentsBookingFactory.getForSupplier(gpSystem)
        factory.generateDefaultAvailableAppointmentSlotExample()
        factory.generateBookingResponse { bookRequest -> bookRequest.respondWithConflictException() }
    }

    @Given("^there are (.*) appointments available to book, but user reached maximum appointment booking limit$")
    fun thereAreAvailableAppointmentsToBookButUserReachedMaximumBookingLimit(gpSystem: String) {

        val factory = AppointmentsBookingFactory.getForSupplier(gpSystem)
        factory.generateDefaultAvailableAppointmentSlotExample()
        factory.generateBookingResponse { bookRequest -> bookRequest.respondWithBookingLimitException() }
    }

    //    EMIS Specific step definition
    @Given("^there are appointments available to book in old EMIS system, " +
            "but user reached maximum appointment booking limit$")
    fun thereAreAvailableAppointmentsToBookInOldEmisSystemButUserReachedMaximumBookingLimit() {
        val factory = AppointmentsBookingFactory.getForSupplier("EMIS")
        factory.generateDefaultAvailableAppointmentSlotExample()
        factory.generateBookingResponse { bookRequest ->
            (bookRequest as BookAppointmentsBuilderEmis)
                    .respondWithBookingLimitExceptionForOldEMIS()
        }
    }

    @Given("^there are EMIS appointments available to book where booking reason is set optional$")
    fun thereAreEMISAppointmentsAvailableToBookWhereBookingReasonIsSetOptional() {
        val factory = AppointmentsBookingFactory.getForSupplier("EMIS")
        factory.generateDefaultAvailableAppointmentSlotExample(reasonNecessityOption = NecessityOption.OPTIONAL)
        factory.generateSuccessfulBookingResponse("")
    }

    @Given("^there are EMIS appointments available to book where booking reason is set optional " +
            "with a reason entered$")
    fun thereAreEMISAppointmentsAvailableToBookWhereBookingReasonIsSetOptionalWithAReasonEntered() {
        val factory = AppointmentsBookingFactory.getForSupplier("EMIS")
        factory.generateDefaultAvailableAppointmentSlotExample(reasonNecessityOption = NecessityOption.OPTIONAL)
        factory.generateSuccessfulBookingResponse()
    }

    @Given("^there are (.*) appointments available to book where booking reason option is set not required$")
    fun thereAreAppointmentsAvailableToBookWhereBookingReasonIsSetRequired(gpSystem: String) {
        val factory = AppointmentsBookingFactory.getForSupplier(gpSystem)
        factory.generateDefaultAvailableAppointmentSlotExample(reasonNecessityOption = NecessityOption.NOT_ALLOWED)
        factory.generateSuccessfulBookingResponse("")
    }

    @Given("^there are appointments available to book which are of telephone type$")
    fun thereAreAvailableAppointmentsToBookWhichAreOfTelephoneType() {
        generateStubsForTelephoneAppointments(NecessityOption.MANDATORY)
    }

    @Given("I wish to book an appointment without specifying a reason$")
    fun iWishToBookAnAppointmentWithoutSpecifyingAReason() {
        emptyReason = true
    }

    @Given("^there are appointments available to book which are of telephone type with " +
            "optional booking reason$")
    fun thereAreAppointmentsAvailableToBookWhichAreOfTelephoneTypeWithOptionalBookingReason() {
        generateStubsForTelephoneAppointments(NecessityOption.OPTIONAL)
    }

    private fun generateStubsForTelephoneAppointments(necessityOption: NecessityOption) {
        val factory = AppointmentsBookingFactory.getForSupplier("EMIS")
        val telephoneNumber = Serenity.sessionVariableCalled<String>(AppointmentsConfirmationSteps
                .SerenityVariable.TELEPHONE_NUMBER_TO_BOOK_AGAINST) ?: "7777777777"

        if (SerenityHelpers.getValueOrNull<String>(AppointmentsBookingFactory.telephoneNumberToEnter) == null) {
            Serenity.setSessionVariable(AppointmentsBookingFactory.telephoneNumberToEnter).to(telephoneNumber)
        }

        factory.generateAvailableSlotExampleIncludingTelephoneAppointment(reasonNecessityOption = necessityOption)

        factory.telephoneAppointmentBookingSetupWithResult(telephoneNumber, emptyReason) { builder ->
            builder
                    .respondWithSuccess()
                    .inScenario("Appointments")
                    .willSetStateTo("Appointment Booked")
        }

        aBookedAppointmentCanBeCancelled()
    }
}
