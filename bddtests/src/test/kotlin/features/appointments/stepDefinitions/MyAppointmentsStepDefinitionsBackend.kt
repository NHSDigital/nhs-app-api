package features.appointments.stepDefinitions

import cucumber.api.java.en.Given
import cucumber.api.java.en.Then
import cucumber.api.java.en.When
import features.appointments.factories.MyAppointmentsFactory
import features.appointments.steps.MyAppointmentsBackendSteps
import mocking.data.appointments.AppointmentsSlotsExample
import mocking.gpServiceBuilderInterfaces.appointments.IMyAppointmentsBuilder
import mockingFacade.appointments.MyAppointmentsFacade
import net.serenitybdd.core.Serenity
import net.thucydides.core.annotations.Steps
import org.junit.Assert
import worker.NhsoHttpException
import worker.WorkerClient
import worker.models.appointments.MyAppointmentsResponse
import java.time.LocalDateTime

class MyAppointmentsStepDefinitionsBackend {

    @Steps
    lateinit var myAppointmentsBackendSteps: MyAppointmentsBackendSteps

    private val appointmentSlotsExample = AppointmentsSlotsExample()

    @Given("^I have no booked appointments for (.*)$")
    fun iHaveNoBookedAppointments(gpService: String) {
        val viewAppointmentFactory = MyAppointmentsFactory.getForSupplier(gpService)
        viewAppointmentFactory.createSuccessfulEmptyMyAppointmentResponse()
    }

    @Given("^I have upcoming appointments before cutoff time for (\\w+)$")
    fun iHaveUpcomingAppointments(gpService: String) {
        val viewAppointmentFactory = MyAppointmentsFactory.getForSupplier(gpService)
        viewAppointmentFactory.createSuccessfulMyAppointmentsResponse()
    }

    @Given("^I have historical appointments for (\\w+)$")
    fun iHaveHistoricalAppointments(gpService: String) {
        val viewAppointmentFactory = MyAppointmentsFactory.getForSupplier(gpService)
        viewAppointmentFactory.createSuccessfulMyAppointmentsResponse(
                appointmentSlotsExample.getGenericExample(
                        arrayListOf(appointmentSlotsExample.getHistoricalAppointmentSession())
                )
        )
    }

    @Given("^I have historical and upcoming appointments for (\\w+)$")
    fun iHaveHistoricalAndUpcomingAppointments(gpService: String) {
        val historicalAppointments = appointmentSlotsExample.getHistoricalAppointmentSession()
        val upcomingAppointments = appointmentSlotsExample.getExampleWithAppointmentWithinCutoffTime()
        val allAppointments = arrayListOf(historicalAppointments, upcomingAppointments)
        val viewAppointmentFactory = MyAppointmentsFactory.getForSupplier(gpService)
        viewAppointmentFactory.createSuccessfulMyAppointmentsResponse(
                appointmentSlotsExample.getGenericExample(allAppointments)
        )
    }

    @Given("^I have upcoming appointments for (\\w+), with one in the past$")
    fun iHaveUpcomingAppointmentsAndOneInThePast(gpService: String) {
        val viewAppointmentFactory = MyAppointmentsFactory.getForSupplier(gpService)
        viewAppointmentFactory.createSuccessfulMyAppointmentsResponse(
                appointmentSlotsExample.getGenericExample(arrayListOf(
                        appointmentSlotsExample.getExampleWithPastAppointment()
                ))
        )
    }

    @Given("^I have upcoming appointments before cutoff time for VISION with only one cancellation reason$")
    fun iHaveUpcomingAppointmentsBeforeCutoffWithOneCancellationReason() {
        val viewAppointmentFactory = MyAppointmentsFactory.getForSupplier("VISION")
        viewAppointmentFactory.createSuccessfulMyAppointmentsResponse(numberOfCancellationReasons = 1)
    }

    @Given("^I have upcoming appointments before cutoff time for VISION without cancellation reasons$")
    fun iHaveUpcomingAppointmentsBeforeCutoffWithoutCancellationReasons() {
        val viewAppointmentFactory = MyAppointmentsFactory.getForSupplier("VISION")
        viewAppointmentFactory.createSuccessfulMyAppointmentsResponse(numberOfCancellationReasons = 0)
    }

    @Given("^I have upcoming appointments within cutoff time for VISION with cancellation reasons$")
    fun iHaveUpcomingAppointmentsWithinCutoffWithOneCancellationReason() {
        val viewAppointmentFactory = MyAppointmentsFactory.getForSupplier("VISION")
        viewAppointmentFactory.createSuccessfulMyAppointmentsResponse(
                appointmentSlotsExample.getGenericExample(
                        arrayListOf(appointmentSlotsExample.getExampleWithAppointmentWithinCutoffTime())),
                2)
    }

    @Given("^I have upcoming appointments within cutoff time for VISION without cancellation reasons$")
    fun iHaveUpcomingAppointmentsWithinCutoffWithoutCancellationReasons() {
        val viewAppointmentFactory = MyAppointmentsFactory.getForSupplier("VISION")
        viewAppointmentFactory.createSuccessfulMyAppointmentsResponse(
                appointmentSlotsExample.getGenericExample(
                        arrayListOf(appointmentSlotsExample.getExampleWithAppointmentWithinCutoffTime())),
                0)
    }

    @Given("^I have upcoming appointments before and within cutoff time for VISION with cancellation reasons$")
    fun iHaveUpcomingAppointmentsBeforeAndWithinCutoffWithOneCancellationReason() {
        val viewAppointmentFactory = MyAppointmentsFactory.getForSupplier("VISION")
        viewAppointmentFactory.createSuccessfulMyAppointmentsResponse(
                appointmentSlotsExample.getGenericExample(
                        arrayListOf(appointmentSlotsExample.getExampleWithAppointmentWithinCutoffTime())),
                2)
    }

    @Given("^a booked appointment cannot be cancelled$")
    fun aBookedAppointmentCannotBeCancelled() {
        val viewAppointmentFactory = MyAppointmentsFactory.getForSupplier("VISION")
        viewAppointmentFactory.createSuccessfulEmptyMyAppointmentResponse(emptyList())
        viewAppointmentFactory.createSuccessfulMyAppointmentsResponseOnceBooked(numberOfCancellationReasons = 0)
    }

    @Given("^(.*) does not offer online booking to my patient$")
    fun appointmentBookingUnavailableToPatientWhenWantingToViewAppointmentSlots(provider: String) {
        val currentViewAppointmentFactory = MyAppointmentsFactory.getForSupplier(provider)
        currentViewAppointmentFactory.createMyAppointments {
            respondWithGPErrorWhenNotEnabled()
        }
    }

    @Given("^(.*) returns corrupted response for my appointments")
    fun corruptedResponseFromMyAppointments(provider: String) {
        val currentViewAppointmentFactory = MyAppointmentsFactory.getForSupplier(provider)
        currentViewAppointmentFactory.createMyAppointments {
            respondWithCorrupted()
        }
    }

    @Given("^(.*) will time out when trying to retrieve my appointments")
    fun timeoutResponseFromMyAppointments(provider: String) {
        val currentViewAppointmentFactory = MyAppointmentsFactory.getForSupplier(provider)
        currentViewAppointmentFactory.createTimeoutMyAppointmentsResponse()
    }

    @Given("^an unknown exception occurs when I want to view my (\\w+) appointments$")
    fun anUnknownExceptionOccursWhenIWantToViewMyEMISAppointments(gpService: String) {
        val viewAppointmentFactory = MyAppointmentsFactory.getForSupplier(gpService)
        viewAppointmentFactory.createMyAppointments {
            respondWithUnknownException()
        }
    }

    @Given("^TPP is unavailable for (.*) appointments$")
    fun tppIsUnavailableForAppointments(appointmentType: String) {
        val viewAppointmentFactory = MyAppointmentsFactory.getForSupplier("TPP")
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
                    respondWithGPServiceUnavailableException()
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
                    respondWithGPServiceUnavailableException()
                }
            }
            "both past and upcoming" -> {
                viewAppointmentFactory.mockMyAppointments(IMyAppointmentsBuilder.AppointmentType.BOTH) {
                    respondWithGPServiceUnavailableException()
                }
            }
            else -> Assert.fail("Unable to parse appointment types of $appointmentType")
        }
    }

    @Given("^the (.*) GP appointment system is unavailable$")
    fun theAppointmentSystemIsUnavailable(gpSystem: String) {
        val currentViewAppointmentFactory = MyAppointmentsFactory.getForSupplier(gpSystem)
        currentViewAppointmentFactory.createMyAppointments {
            respondWithGPServiceUnavailableException()
        }
    }

    @When("^my appointments are requested$")
    fun whenTheAPIRetrievesMyAppointments() {
        myAppointmentsBackendSteps.createSerenityMyAppointmentSessionVariable()
    }

    @When("^the \"([^\"]*)\" API call fails with csrf token of \"([^\"]*)\"$")
    fun whenTheAPICallFailsWithCsrfTokenOf(provider: String, csrfToken: String) {
        Assert.assertEquals("Test setup incorrect: Step only implemented for EMIS", "EMIS", provider.toUpperCase())

        try {
            val result = Serenity
                    .sessionVariableCalled<WorkerClient>(WorkerClient::class)
                    .appointments.setCsrfToken(csrfToken).getMyAppointments(LocalDateTime.now().toString())
            Serenity.setSessionVariable(MyAppointmentsResponse::class.java).to(result)
            Assert.fail("The API did not fail with invalid token.")
        } catch (exception: NhsoHttpException) {
            Serenity.setSessionVariable("HttpException").to(exception)
        }
    }

    @Then("^I will only receive upcoming appointments$")
    fun iWillOnlyReceiveUpcomingAppointments() {
        myAppointmentsBackendSteps.checkUpcomingAppointments()
        myAppointmentsBackendSteps.checkHistoricalAppointments(false)
    }

    @Then("^I will only receive historical appointments$")
    fun iWillOnlyReceiveHistoricalAppointments() {
        myAppointmentsBackendSteps.checkUpcomingAppointments(false)
        myAppointmentsBackendSteps.checkHistoricalAppointments()
    }

    @Then("^I will receive both historical and upcoming appointments$")
    fun iWillReceiveBothHistoricalAndUpcomingAppointments() {
        myAppointmentsBackendSteps.checkUpcomingAppointments()
        myAppointmentsBackendSteps.checkHistoricalAppointments()
    }

    @Then("^I will receive no appointments$")
    fun iReceiveNoAppointments() {
        myAppointmentsBackendSteps.checkUpcomingAppointments(false)
        myAppointmentsBackendSteps.checkHistoricalAppointments(false)
    }

    @Then("^I will receive upcoming appointments with appointments in the past$")
    fun iWillReceiveUpcomingAppointmentsInThePast() {
        myAppointmentsBackendSteps.checkUpcomingAppointments()
    }

    @Then("^a list of cancellation reasons if the GP Service provides the list$")
    fun thenAListOfCancellationReasons() {
        myAppointmentsBackendSteps.checkCancellationReasonExistForApplicableGPService()
    }
}
