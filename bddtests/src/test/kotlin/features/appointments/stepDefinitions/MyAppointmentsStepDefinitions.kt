package features.appointments.stepDefinitions

import cucumber.api.java.en.Given
import cucumber.api.java.en.Then
import cucumber.api.java.en.When
import features.appointments.factories.MyAppointmentsFactory
import features.appointments.steps.AppointmentsConfirmationSteps
import features.appointments.steps.MyAppointmentsBackendSteps
import features.appointments.steps.MyAppointmentsUISteps
import mocking.data.appointments.AppointmentsSlotsExample
import net.serenitybdd.core.Serenity
import net.serenitybdd.core.pages.WebElementFacade
import net.thucydides.core.annotations.Steps
import org.junit.Assert
import pages.navigation.HeaderNative
import worker.NhsoHttpException
import worker.WorkerClient
import worker.models.appointments.MyAppointmentsResponse
import java.time.LocalDateTime

class MyAppointmentsStepDefinitions {

    @Steps
    lateinit var myAppointmentsUISteps: MyAppointmentsUISteps

    @Steps
    lateinit var myAppointmentsBackendSteps: MyAppointmentsBackendSteps

    lateinit var headerNative: HeaderNative
    @Steps
    lateinit var appointmentsConfirmationSteps: AppointmentsConfirmationSteps

    @Then("^the Appointment Slot page is displayed$")
    fun theAppointmentSlotPageIsDisplayed() {
        headerNative.waitForPageHeaderText("Confirm appointment")
        appointmentsConfirmationSteps.checkAppointmentDetails()
    }

    @Then("^the Appointment Booking success message is displayed$")
    fun appointmentBookingSuccessMessage() {
        myAppointmentsUISteps.checkBookingSuccessMessage()
    }

    @Then("^the booked appointment before cutoff time is correctly displayed with ability to cancel$")
    fun bookedAppointmentIsCorrectlyDisplayedWithCancel() {
        myAppointmentsUISteps.checkUpcomingAppointmentsAreCorrectlyPopulated()
        myAppointmentsUISteps.verifyThatThereIsACancelLinkForEachUpcomingAppointment()
    }

    @Then("^booked appointments before and one appointment within cutoff time " +
            "are correctly displayed with relevant ability to cancel$")
    fun bookedAppointmentIsCorrectlyDisplayedWithCancelExceptOnesWithinCutoffTime() {
        myAppointmentsUISteps.checkUpcomingAppointmentsAreCorrectlyPopulated()
        myAppointmentsUISteps.verifyThatThereIsACancelLinkForEachUpcomingAppointment(1)
    }

    @Then("^the Appointment Booking success message is displayed without reference to being able to cancel$")
    fun appointmentBookingConfirmationScreenIsDisplayedWithoutReferenceToCancel() {
        myAppointmentsUISteps.checkBookingSuccessMessage(false)

    }

    @Then("^the booked appointment is correctly displayed without ability to cancel$")
    fun bookedAppointmentIsCorrectlyDisplayedWithoutCancel() {
        myAppointmentsUISteps.checkUpcomingAppointmentsAreCorrectlyPopulated()
        myAppointmentsUISteps.verifyThatThereAreNoCancelLinks()
    }

    @Then("^I can book an appointment$")
    fun iCanBookAnAppointment() {
        myAppointmentsUISteps.myAppointmentsPage.bookButton.assertSingleElementPresent().assertIsVisible()
        myAppointmentsUISteps.myAppointmentsPage.bookButton.element.waitUntilPresent<WebElementFacade>()

        Assert.assertTrue("Book an appointment is not displaying",
                myAppointmentsUISteps.myAppointmentsPage.bookButton.element.isDisplayed)

        Assert.assertTrue("Book an appointment is not enabled",
                myAppointmentsUISteps.myAppointmentsPage.bookButton.element.isCurrentlyEnabled)
    }

    @Then("^the page title is \"My appointments\"$")
    fun thePageTitleIsMyAppointments() {
        myAppointmentsUISteps.checkHeaderTextIsCorrect()
    }

    @Then("^the My Appointments page is displayed$")
    fun iWillBeOnTheMyAppointmentsScreen() {
        iCanBookAnAppointment()
        thePageTitleIsMyAppointments()
    }

    @Then("^I am informed I have no upcoming appointments$")
    fun thenIAmInformedIHaveNoBookedAppointments() {
        myAppointmentsUISteps.checkNoUpcomingAppointmentsTextIsDisplaying()
    }

    @Then("^I am informed I have no historical appointments$")
    fun thenIAmInformedIHaveNoHistoricalAppointments() {
        myAppointmentsUISteps.checkNoHistoricalAppointmentsTextIsDisplaying()
    }

    @Then("^I am not informed I have no historical appointments$")
    fun thenIAmNotInformedIHaveNoHistoricalAppointments() {
        myAppointmentsUISteps.myAppointmentsPage.assertPastTextNotPresent()
    }

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
                AppointmentsSlotsExample.getGenericExample(
                        arrayListOf(AppointmentsSlotsExample.historicalAppointmentSession)
                )
        )
    }

    @Given("^I have historical and upcoming appointments for (\\w+)$")
    fun iHaveHistoricalAndUpcomingAppointments(gpService: String) {
        val viewAppointmentFactory = MyAppointmentsFactory.getForSupplier(gpService)
        viewAppointmentFactory.createSuccessfulMyAppointmentsResponse(
                AppointmentsSlotsExample.getGenericExample(
                        arrayListOf(
                                AppointmentsSlotsExample.historicalAppointmentSession,
                                AppointmentsSlotsExample.appointmentSessionWithinCutoffTime
                        )
                )
        )
    }

    @Given("^I have upcoming appointments for (\\w+), with one in the past$")
    fun iHaveUpcomingAppointmentsAndOneInThePast(gpService: String) {
        val viewAppointmentFactory = MyAppointmentsFactory.getForSupplier(gpService)
        viewAppointmentFactory.createSuccessfulMyAppointmentsResponse(
                AppointmentsSlotsExample.getExampleWithPastAppointment()
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
                AppointmentsSlotsExample.getGenericExample(
                        arrayListOf(AppointmentsSlotsExample.appointmentSessionWithinCutoffTime)),
                2)
    }

    @Given("^I have upcoming appointments within cutoff time for VISION without cancellation reasons$")
    fun iHaveUpcomingAppointmentsWithinCutoffWithoutCancellationReasons() {
        val viewAppointmentFactory = MyAppointmentsFactory.getForSupplier("VISION")
        viewAppointmentFactory.createSuccessfulMyAppointmentsResponse(
                AppointmentsSlotsExample.getGenericExample(
                        arrayListOf(AppointmentsSlotsExample.appointmentSessionWithinCutoffTime)),
                0)
    }

    @Given("^I have upcoming appointments before and within cutoff time for VISION with cancellation reasons$")
    fun iHaveUpcomingAppointmentsBeforeAndWithinCutoffWithOneCancellationReason() {
        val viewAppointmentFactory = MyAppointmentsFactory.getForSupplier("VISION")
        viewAppointmentFactory.createSuccessfulMyAppointmentsResponse(
                AppointmentsSlotsExample.getExampleWithAppointmentWithinCutoffTime(),
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

    @When("^I select \"([^\"]*)\" button$")
    fun whenISelectButton(buttonText: String) {
        myAppointmentsUISteps.myAppointmentsPage.waitForNativeStepToComplete()
        myAppointmentsUISteps.myAppointmentsPage.clickOnButtonContainingText(buttonText)
    }

    @Then("^I am given the list of upcoming appointments$")
    fun thenIAmGivenTheListOfUpcomingAppointments() {
        myAppointmentsUISteps.checkUpcomingAppointmentsAreCorrectlyPopulated()
        myAppointmentsUISteps.checkIfUpcomingSlotsAreInCorrectOrder()
    }

    @Then("^I am given the list of historical appointments$")
    fun thenIAmGivenTheListOfHistoricalAppointments() {
        myAppointmentsUISteps.checkHistoricalAppointmentsAreCorrectlyPopulated()
        myAppointmentsUISteps.checkIfHistoricalSlotsAreInCorrectOrder()
    }

    @Then("^each appointment can be cancelled$")
    fun eachAppointmentCanBeCancelled() {
        Assert.assertEquals(
                "Missing at least one cancel link. ",
                myAppointmentsUISteps.myAppointmentsPage.getWebAppointmentSlotDivs().size,
                myAppointmentsUISteps.myAppointmentsPage.getNumberOfCancelLinks()
        )
    }

    @Then("^no appointment can be cancelled$")
    fun noAppointmentCanBeCancelled() {
        myAppointmentsUISteps.verifyThatThereAreNoCancelLinks()
    }

    @When("^the upcoming appointments are requested$")
    fun whenTheAPIRetrievesUpcomingAppointments() {
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

    @When("^I select a \"Cancel appointment\" link$")
    fun iSelectACancelLink() {
        myAppointmentsUISteps.myAppointmentsPage.clickFirstCancelAppointmentLink()
    }

    @Then("^a \"Cancellation confirmed\" message is displayed$")
    fun cancellationConfirmationMessage() {
        myAppointmentsUISteps.verifyCancellationConfirmationMessage()
    }

    @Given("^the (.*) GP appointment system is unavailable$")
    fun theAppointmentSystemIsUnavailable(gpSystem: String) {
        val currentViewAppointmentFactory = MyAppointmentsFactory.getForSupplier(gpSystem)
        currentViewAppointmentFactory.createMyAppointments {
            respondWithGPServiceUnavailableException()
        }
    }

    @Then("^I see page header indicating there is an appointment data error$")
    fun iSeePageHeaderIndicatingAppointmentDataError() {
        myAppointmentsUISteps.headerNative.waitForPageHeaderText("Appointment data error")
    }

    @Then("^I see the appropriate error messages for the appointment data error$")
    fun iSeeTheAppropriateErrorMessagesForTheAppointmentDataError() {
        myAppointmentsUISteps.checkAppointmentDataErrorMessagesAreCorrect()
    }
}
