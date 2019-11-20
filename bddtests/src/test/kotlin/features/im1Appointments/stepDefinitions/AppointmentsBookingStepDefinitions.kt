package features.im1Appointments.stepDefinitions

import cucumber.api.java.en.Given
import mocking.emis.appointments.BookAppointmentsBuilderEmis
import mocking.emis.practices.NecessityOption
import mocking.stubs.appointments.factories.AppointmentsBookingFactory
import mocking.stubs.appointments.factories.AppointmentsBookingFactory.Companion.symptomsToEnter
import mocking.stubs.appointments.factories.MyAppointmentsFactory
import net.serenitybdd.core.Serenity
import utils.GlobalSerenityHelpers
import utils.getOrFail
import java.time.Duration

private const val DELAY_IN_SECONDS = 12L
private const val DEFAULT_BOOKING_REASON = "I would like to see a doctor"

class AppointmentsBookingStepDefinitions {

    @Given("^there are (.*) appointments available to book$")
    fun thereAreAvailableAppointmentsToBook(gpSystem: String) {
        val factory = AppointmentsBookingFactory.getForSupplier(gpSystem)
        factory.generateDefaultAvailableAppointmentSlotExample()
        factory.generateSuccessfulBookingResponse()
    }

    @Given("^a booked appointment can be cancelled$")
    fun aBookedAppointmentCanBeCancelled() {
        val gpSystem = GlobalSerenityHelpers.GP_SYSTEM.getOrFail<String>()
        val viewAppointmentFactory = MyAppointmentsFactory.getForSupplier(gpSystem)
        viewAppointmentFactory.createSuccessfulMyAppointmentsResponseOnceBooked()
    }

    @Given("^there are multiple appointment slots at the same time, provided by (.*)$")
    fun thereAreAvailableAppointmentsAtTheSameTime(gpSystem: String) {
        val factory = AppointmentsBookingFactory.getForSupplier(gpSystem)
        factory.generateMultipleAvailableAppointmentSlots()
        factory.generateSuccessfulBookingResponse()
    }

    @Given("^there are (.*) appointments available to book with a reason")
    fun thereAreAvailableAppointmentsToBookWithAReason(gpSystem: String) {
        val bookingReasonOfSpecifiedLength = DEFAULT_BOOKING_REASON
        Serenity.setSessionVariable(symptomsToEnter).to(DEFAULT_BOOKING_REASON)
        val factory = AppointmentsBookingFactory.getForSupplier(gpSystem)
        factory.generateDefaultAvailableAppointmentSlotExample()
        factory.generateSuccessfulBookingResponse(bookingReasonOfSpecifiedLength)
    }

    @Given("^there are (.*) appointments available to book and user attempts to enter a dangerous booking reason$")
    fun thereAreAvailableAppointmentsToBookWithADangerousBookingReason(gpSystem: String) {
        val factory = AppointmentsBookingFactory.getForSupplier(gpSystem)
        val bookingReason = "<script>"
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
        factory.generateBookingResponse { bookRequest -> bookRequest.respondWithServiceUnavailable() }
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
}
