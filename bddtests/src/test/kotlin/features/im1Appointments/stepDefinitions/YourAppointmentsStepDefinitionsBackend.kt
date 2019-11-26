package features.im1Appointments.stepDefinitions

import constants.Supplier
import cucumber.api.java.en.Given
import cucumber.api.java.en.Then
import cucumber.api.java.en.When
import mocking.stubs.appointments.factories.MyAppointmentsFactory
import features.im1Appointments.steps.YourAppointmentsBackendSteps
import features.linkedProfiles.LinkedProfilesSerenityHelpers
import mocking.data.appointments.AppointmentSlotsTelephoneExample
import mocking.data.appointments.AppointmentsSlotsExample
import mocking.gpServiceBuilderInterfaces.appointments.IMyAppointmentsBuilder
import mockingFacade.appointments.MyAppointmentsFacade
import net.serenitybdd.core.Serenity
import net.thucydides.core.annotations.Steps
import org.junit.Assert
import utils.getOrFail
import worker.NhsoHttpException
import worker.WorkerClient
import worker.models.appointments.MyAppointmentsResponse
import java.time.LocalDateTime

class YourAppointmentsStepDefinitionsBackend {

    @Steps
    lateinit var yourAppointmentsBackendSteps: YourAppointmentsBackendSteps

    private val appointmentSlotsExample = AppointmentsSlotsExample()
    private val appointmentSlotsTelephoneExample = AppointmentSlotsTelephoneExample()

    @Given("^I have no booked appointments for (.*)$")
    fun iHaveNoBookedAppointments(gpSystem: String) {
        val supplier = Supplier.valueOf(gpSystem)
        val viewAppointmentFactory = MyAppointmentsFactory.getForSupplier(supplier)
        viewAppointmentFactory.createSuccessfulEmptyMyAppointmentResponse()
    }

    @Given("^I have upcoming appointments before cutoff time for (\\w+)$")
    fun iHaveUpcomingAppointments(gpSystem: String) {
        val supplier = Supplier.valueOf(gpSystem)
        val viewAppointmentFactory = MyAppointmentsFactory.getForSupplier(supplier)
        viewAppointmentFactory.createSuccessfulMyAppointmentsResponse()
    }

    @Given("^I have upcoming telephone appointments before cutoff time for (\\w+)$")
    fun iHaveUpcomingTelephoneAppointments(gpSystem: String) {
        val supplier = Supplier.valueOf(gpSystem)
        val viewAppointmentFactory = MyAppointmentsFactory.getForSupplier(supplier)
        viewAppointmentFactory.createSuccessfulMyTelephoneAppointmentsResponse()
    }

    @Given("^I have historical telephone appointments for (\\w+)$")
    fun iHaveHistoricalTelephoneAppointments(gpSystem: String) {
        val supplier = Supplier.valueOf(gpSystem)
        val viewAppointmentFactory = MyAppointmentsFactory.getForSupplier(supplier)
        viewAppointmentFactory.createSuccessfulMyTelephoneAppointmentsResponse(
                appointmentSlotsExample.getGenericExample(
                        arrayListOf(appointmentSlotsTelephoneExample.getHistoricalTelephoneAppointmentSession())
                ))
    }

    @Given("^I have historical appointments for (\\w+)$")
    fun iHaveHistoricalAppointments(gpSystem: String) {
        val supplier = Supplier.valueOf(gpSystem)
        val viewAppointmentFactory = MyAppointmentsFactory.getForSupplier(supplier)
        viewAppointmentFactory.createSuccessfulMyAppointmentsResponse(
                appointmentSlotsExample.getGenericExample(
                        arrayListOf(appointmentSlotsExample.getHistoricalAppointmentSession())
                )
        )
    }

    @Given("^I have historical and upcoming appointments for (\\w+)$")
    fun iHaveHistoricalAndUpcomingAppointments(gpSystem: String) {
        val supplier = Supplier.valueOf(gpSystem)
        val historicalAppointments = appointmentSlotsExample.getHistoricalAppointmentSession()
        val upcomingAppointments = appointmentSlotsExample.getExampleWithAppointmentWithinCutoffTime()
        val allAppointments = arrayListOf(historicalAppointments, upcomingAppointments)
        val viewAppointmentFactory = MyAppointmentsFactory.getForSupplier(supplier)
        viewAppointmentFactory.createSuccessfulMyAppointmentsResponse(
                appointmentSlotsExample.getGenericExample(allAppointments)
        )
    }

    @Given("^I have upcoming appointments for (\\w+), with one in the past$")
    fun iHaveUpcomingAppointmentsAndOneInThePast(gpSystem: String) {
        val supplier = Supplier.valueOf(gpSystem)
        val viewAppointmentFactory = MyAppointmentsFactory.getForSupplier(supplier)
        viewAppointmentFactory.createSuccessfulMyAppointmentsResponse(
                appointmentSlotsExample.getGenericExample(arrayListOf(
                        appointmentSlotsExample.getExampleWithPastAppointment()
                ))
        )
    }

    @Given("^I have upcoming appointments before cutoff time for VISION with only one cancellation reason$")
    fun iHaveUpcomingAppointmentsBeforeCutoffWithOneCancellationReason() {
        val viewAppointmentFactory = MyAppointmentsFactory.getForSupplier(Supplier.VISION)
        viewAppointmentFactory.createSuccessfulMyAppointmentsResponse(numberOfCancellationReasons = 1)
    }

    @Given("^I have upcoming appointments before cutoff time for VISION without cancellation reasons$")
    fun iHaveUpcomingAppointmentsBeforeCutoffWithoutCancellationReasons() {
        val viewAppointmentFactory = MyAppointmentsFactory.getForSupplier(Supplier.VISION)
        viewAppointmentFactory.createSuccessfulMyAppointmentsResponse(numberOfCancellationReasons = 0)
    }

    @Given("^I have upcoming appointments within cutoff time for VISION with cancellation reasons$")
    fun iHaveUpcomingAppointmentsWithinCutoffWithOneCancellationReason() {
        val viewAppointmentFactory = MyAppointmentsFactory.getForSupplier(Supplier.VISION)
        viewAppointmentFactory.createSuccessfulMyAppointmentsResponse(
                appointmentSlotsExample.getGenericExample(
                        arrayListOf(appointmentSlotsExample.getExampleWithAppointmentWithinCutoffTime())),
                2)
    }

    @Given("^I have upcoming appointments within cutoff time for VISION without cancellation reasons$")
    fun iHaveUpcomingAppointmentsWithinCutoffWithoutCancellationReasons() {
        val viewAppointmentFactory = MyAppointmentsFactory.getForSupplier(Supplier.VISION)
        viewAppointmentFactory.createSuccessfulMyAppointmentsResponse(
                appointmentSlotsExample.getGenericExample(
                        arrayListOf(appointmentSlotsExample.getExampleWithAppointmentWithinCutoffTime())),
                0)
    }

    @Given("^I have upcoming appointments before and within cutoff time for VISION with cancellation reasons$")
    fun iHaveUpcomingAppointmentsBeforeAndWithinCutoffWithOneCancellationReason() {
        val viewAppointmentFactory = MyAppointmentsFactory.getForSupplier(Supplier.VISION)
        viewAppointmentFactory.createSuccessfulMyAppointmentsResponse(
                appointmentSlotsExample.getGenericExample(
                        arrayListOf(appointmentSlotsExample.getExampleWithAppointmentWithinCutoffTime())),
                2)
    }

    @Given("^a booked appointment cannot be cancelled$")
    fun aBookedAppointmentCannotBeCancelled() {
        val viewAppointmentFactory = MyAppointmentsFactory.getForSupplier(Supplier.VISION)
        viewAppointmentFactory.createSuccessfulEmptyMyAppointmentResponse(emptyList())
        viewAppointmentFactory.createSuccessfulMyAppointmentsResponseOnceBooked(numberOfCancellationReasons = 0)
    }

    @Given("^(.*) does not offer online booking to my patient$")
    fun appointmentBookingUnavailableToPatientWhenWantingToViewAppointmentSlots(gpSystem: String) {
        val supplier = Supplier.valueOf(gpSystem)
        val currentViewAppointmentFactory = MyAppointmentsFactory.getForSupplier(supplier)
        currentViewAppointmentFactory.createMyAppointments {
            respondWithGPErrorWhenNotEnabled()
        }
    }

    @Given("^(.*) returns corrupted response for my appointments")
    fun corruptedResponseFromMyAppointments(gpSystem: String) {
        val supplier = Supplier.valueOf(gpSystem)
        val currentViewAppointmentFactory = MyAppointmentsFactory.getForSupplier(supplier)
        currentViewAppointmentFactory.createMyAppointments {
            respondWithCorrupted()
        }
    }

    @Given("^(.*) will time out when trying to retrieve my appointments")
    fun timeoutResponseFromMyAppointments(gpSystem: String) {
        val supplier = Supplier.valueOf(gpSystem)
        val currentViewAppointmentFactory = MyAppointmentsFactory.getForSupplier(supplier)
        currentViewAppointmentFactory.createTimeoutMyAppointmentsResponse()
    }

    @Given("^an unknown exception occurs when I want to view my (\\w+) appointments$")
    fun anUnknownExceptionOccursWhenIWantToViewMyEMISAppointments(gpSystem: String) {
        val supplier = Supplier.valueOf(gpSystem)
        val viewAppointmentFactory = MyAppointmentsFactory.getForSupplier(supplier)
        viewAppointmentFactory.createMyAppointments {
            respondWithUnknownException()
        }
    }

    @Given("^TPP is unavailable for (.*) appointments$")
    fun tppIsUnavailableForAppointments(appointmentType: String) {
        val viewAppointmentFactory = MyAppointmentsFactory.getForSupplier(Supplier.TPP)
        val example = MyAppointmentsFacade(
                appointmentSlotsExample.getGenericExample(
                        arrayListOf(
                                appointmentSlotsExample.getHistoricalAppointmentSession(),
                                appointmentSlotsExample.getExampleWithAppointmentWithinCutoffTime()
                        )
                )
        )
        when (appointmentType) {
            "past" -> {
                viewAppointmentFactory.mockMyAppointments(IMyAppointmentsBuilder.AppointmentType.PAST_ONLY) {
                    respondWithServiceUnavailable()
                }
                viewAppointmentFactory.mockMyAppointments(IMyAppointmentsBuilder.AppointmentType.UPCOMING_ONLY) {
                    respondWithSuccess(example)
                }
            }
            "upcoming" -> {
                viewAppointmentFactory.mockMyAppointments(IMyAppointmentsBuilder.AppointmentType.PAST_ONLY) {
                    respondWithSuccess(example)
                }
                viewAppointmentFactory.mockMyAppointments(IMyAppointmentsBuilder.AppointmentType.UPCOMING_ONLY) {
                    respondWithServiceUnavailable()
                }
            }
            "both past and upcoming" -> {
                viewAppointmentFactory.mockMyAppointments(IMyAppointmentsBuilder.AppointmentType.BOTH) {
                    respondWithServiceUnavailable()
                }
            }
            else -> Assert.fail("Unable to parse appointment types of $appointmentType")
        }
    }

    @Given("^the (.*) GP appointment system is unavailable$")
    fun theAppointmentSystemIsUnavailable(gpSystem: String) {
        val supplier = Supplier.valueOf(gpSystem)
        val currentViewAppointmentFactory = MyAppointmentsFactory.getForSupplier(supplier)
        currentViewAppointmentFactory.createMyAppointments {
            respondWithServiceUnavailable()
        }
    }

    @When("^my appointments are requested$")
    fun whenTheAPIRetrievesMyAppointments() {
        yourAppointmentsBackendSteps.createSerenityMyAppointmentSessionVariable()
    }

    @When("^the \"([^\"]*)\" API call fails with csrf token of \"([^\"]*)\"$")
    fun whenTheAPICallFailsWithCsrfTokenOf(provider: String, csrfToken: String) {
        Assert.assertEquals("Test setup incorrect: Step only implemented for EMIS", "EMIS",
                provider.toUpperCase())

        try {
            val patientId = LinkedProfilesSerenityHelpers.MAIN_PATIENT_ID.getOrFail<String>()
            val result = Serenity
                    .sessionVariableCalled<WorkerClient>(WorkerClient::class)
                    .appointments.setCsrfToken(csrfToken).getMyAppointments(patientId, LocalDateTime.now().toString())
            Serenity.setSessionVariable(MyAppointmentsResponse::class.java).to(result)
            Assert.fail("The API did not fail with invalid token.")
        } catch (exception: NhsoHttpException) {
            Serenity.setSessionVariable("HttpException").to(exception)
        }
    }

    @Then("^I will only receive upcoming appointments$")
    fun iWillOnlyReceiveUpcomingAppointments() {
        yourAppointmentsBackendSteps.checkUpcomingAppointments()
        yourAppointmentsBackendSteps.checkHistoricalAppointments(false)
    }

    @Then("^I will only receive historical appointments$")
    fun iWillOnlyReceiveHistoricalAppointments() {
        yourAppointmentsBackendSteps.checkUpcomingAppointments(false)
        yourAppointmentsBackendSteps.checkHistoricalAppointments()
    }

    @Then("^I will receive both historical and upcoming appointments$")
    fun iWillReceiveBothHistoricalAndUpcomingAppointments() {
        yourAppointmentsBackendSteps.checkUpcomingAppointments()
        yourAppointmentsBackendSteps.checkHistoricalAppointments()
    }

    @Then("^I will receive no appointments$")
    fun iReceiveNoAppointments() {
        yourAppointmentsBackendSteps.checkUpcomingAppointments(false)
        yourAppointmentsBackendSteps.checkHistoricalAppointments(false)
    }

    @Then("^I will receive upcoming appointments with appointments in the past$")
    fun iWillReceiveUpcomingAppointmentsInThePast() {
        yourAppointmentsBackendSteps.checkUpcomingAppointments()
    }

    @Then("^a list of cancellation reasons if the GP Service provides the list$")
    fun thenAListOfCancellationReasons() {
        yourAppointmentsBackendSteps.checkCancellationReasonExistForApplicableGPService()
    }
}
