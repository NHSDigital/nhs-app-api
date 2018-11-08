package features.appointments.stepDefinitions

import cucumber.api.java.en.Given
import cucumber.api.java.en.Then
import cucumber.api.java.en.When
import features.appointments.factories.UpcomingAppointmentsFactory
import features.appointments.steps.MyAppointmentsSteps
import mocking.data.appointments.AppointmentsSlotsExample
import net.serenitybdd.core.Serenity
import net.thucydides.core.annotations.Steps
import org.junit.Assert
import pages.navigation.HeaderNative
import worker.NhsoHttpException
import worker.WorkerClient
import worker.models.appointments.MyAppointmentsResponse
import java.time.LocalDateTime

class MyAppointmentsStepDefinitions {

    @Steps
    lateinit var myAppointmentsSteps: MyAppointmentsSteps
    lateinit var headerNative: HeaderNative


    @Then("^the Appointment Slot page is displayed$")
    fun theAppointmentSlotPageIsDisplayed() {
        headerNative.waitForPageHeaderText("Confirm appointment")
    }

    @Then("^the Appointment Booking success message is displayed$")
    fun appointmentBookingSuccessMessage() {
        myAppointmentsSteps.checkBookingSuccessMessage()
    }

    @Then("^the booked appointment is correctly displayed with ability to cancel$")
    fun bookedAppointmentIsCorrectlyDisplayedWithCancel() {
        myAppointmentsSteps.checkAppointmentsExistAndAppointmentDataAreCorrectlyPopulated()
        myAppointmentsSteps.verifyThatThereIsACancelLinkForEachUpcomingAppointment()
    }

    @Then("^the Appointment Booking success message is displayed without reference to being able to cancel$")
    fun appointmentBookingConfirmationScreenIsDisplayedWithoutReferenceToCancel() {
        myAppointmentsSteps.checkBookingSuccessMessage(false)

    }

    @Then("^the booked appointment is correctly displayed without ability to cancel$")
    fun bookedAppointmentIsCorrectlyDisplayedWithoutCancel() {
        myAppointmentsSteps.checkAppointmentsExistAndAppointmentDataAreCorrectlyPopulated()
        myAppointmentsSteps.verifyThatThereAreNoCancelLinks()
    }

    @Then("^I can book an appointment$")
    fun iCanBookAnAppointment() {
        myAppointmentsSteps.checkIfBookAnAppointmentButtonExistAndEnabled()
    }

    @Then("^the page title is \"My appointments\"$")
    fun thePageTitleIsMyAppointments() {
        myAppointmentsSteps.checkHeaderTextIsCorrect()
    }

    @Then("^I will be on the My appointments screen$")
    fun iWillBeOnTheMyAppointmentsScreen() {
        iCanBookAnAppointment()
        thePageTitleIsMyAppointments()
    }

    @Then("^I am informed I have no booked appointments$")
    fun thenIAmInformedIHaveNoBookedAppointments() {
        myAppointmentsSteps.checkNoUpcomingAppointmentsTextIsDisplaying()
    }

    @Given("^I have no upcoming appointments for (.*)$")
    fun iHaveNoUpcomingAppointments(gpService: String) {
        val viewAppointmentFactory = UpcomingAppointmentsFactory.getForSupplier(gpService)
        viewAppointmentFactory.createSuccessfulEmptyUpcomingAppointmentResponse()
    }

    @Given("^I have upcoming appointments for (\\w+)$")
    fun iHaveUpcomingAppointments(gpService: String) {
        val viewAppointmentFactory = UpcomingAppointmentsFactory.getForSupplier(gpService)
        viewAppointmentFactory.createSuccessfulUpcomingAppointmentsResponse()
    }

    @Given("^I have upcoming appointments for (\\w+), with one in the past$")
    fun iHaveUpcomingAppointmentsAndOneInThePast(gpService: String) {
        val viewAppointmentFactory = UpcomingAppointmentsFactory.getForSupplier(gpService)
        viewAppointmentFactory.createSuccessfulUpcomingAppointmentsResponse(
                AppointmentsSlotsExample.getFacadeWithPastAppointment()
        )
    }

    @Given("^I have upcoming appointments for VISION, but with only one cancellation reason$")
    fun iHaveUpcomingAppointmentsWithOneCancellationReason() {
        val viewAppointmentFactory = UpcomingAppointmentsFactory.getForSupplier("VISION")
        viewAppointmentFactory.createSuccessfulUpcomingAppointmentsResponse(numberOfCancellationReasons = 1)
    }

    @Given("^I have upcoming appointments for VISION, but without cancellation reasons$")
    fun iHaveUpcomingAppointmentsWithoutCancellationReasons() {
        val viewAppointmentFactory = UpcomingAppointmentsFactory.getForSupplier("VISION")
        viewAppointmentFactory.createSuccessfulUpcomingAppointmentsResponse(numberOfCancellationReasons = 0)
    }

    @Given("^a booked appointment cannot be cancelled$")
    fun aBookedAppointmentCannotBeCancelled() {
        val viewAppointmentFactory = UpcomingAppointmentsFactory.getForSupplier("VISION")
        viewAppointmentFactory.createSuccessfulEmptyUpcomingAppointmentResponse(emptyList())
        viewAppointmentFactory.createSuccessfulUpcomingAppointmentsResponseOnceBooked(numberOfCancellationReasons = 0)
    }

    @Given("^(.*) does not offer online booking to my patient$")
    fun appointmentBookingUnavailableToPatientWhenWantingToViewAppointmentSlots(provider: String) {
        myAppointmentsSteps.generateStubsForMyAppointmentsWhenUnavailableToPatient(provider)
    }

    @Given("^(.*) returns corrupted response for my appointments")
    fun corruptedResponseFromMyAppointments(provider: String) {
        myAppointmentsSteps.generateCorruptedStubForMyAppointment(provider)
    }

    @Given("^(.*) will time out when trying to retrieve my appointments")
    fun timeoutResponseFromMyAppointments(provider: String) {
        myAppointmentsSteps.generateTimeoutStubForMyAppointment(provider)
    }

    @Given("^an unknown exception occurs when I want to view my (\\w+) appointments$")
    fun anUnknownExceptionOccursWhenIWantToViewMyEMISAppointments(gpService: String) {
        val viewAppointmentFactory = UpcomingAppointmentsFactory.getForSupplier(gpService)
        viewAppointmentFactory.createUpcomingAppointments {
            respondWithUnknownException()
        }
    }

    @When("^I select \"([^\"]*)\" button$")
    fun whenISelectButton(buttonText: String) {
        myAppointmentsSteps.myAppointmentsPage.waitForNativeStepToComplete()
        myAppointmentsSteps.myAppointmentsPage.clickOnButtonContainingText(buttonText)
    }

    @Then("^I am given the list of upcoming appointments$")
    fun thenIAmGivenTheListOfUpcomingAppointments() {
        myAppointmentsSteps.checkAppointmentsExistAndAppointmentDataAreCorrectlyPopulated()
    }

    @Then("^appointments are in chronological order$")
    fun thenAppointmentsAreInChronologicalOrder() {
        myAppointmentsSteps.checkIfSlotsAreInCorrectOrder()
    }

    @Then("^each appointment can be cancelled$")
    fun eachAppointmentCanBeCancelled() {
        myAppointmentsSteps.verifyThatThereIsACancelLinkForEachUpcomingAppointment()
    }

    @Then("^no appointment can be cancelled$")
    fun noAppointmentCanBeCancelled() {
        myAppointmentsSteps.verifyThatThereAreNoCancelLinks()
    }

    @When("^the upcoming appointments are requested$")
    fun whenTheAPIRetrievesUpcomingAppointments() {
        myAppointmentsSteps.createSerenityMyAppointmentSessionVariable()
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
        myAppointmentsSteps.checkMyAppointments()
    }

    @Then("^I will receive upcoming appointments with appointments in the past$")
    fun iWillReceiveUpcomingAppointmentsInThePast() {
        myAppointmentsSteps.checkMyAppointments()
    }

    @Then("^a list of cancellation reasons if the GP Service provides the list$")
    fun thenAListOfCancellationReasons() {
        myAppointmentsSteps.checkCancellationReasonExistForApplicableGPService()
    }

    @When("^I select a \"Cancel appointment\" link$")
    fun iSelectACancelLink() {
        myAppointmentsSteps.clickFirstCancelLink()
    }

    @Then("^a \"Cancellation confirmed\" message is displayed$")
    fun cancellationConfirmationMessage() {
        myAppointmentsSteps.verifyCancellationConfirmationMessage()
    }

    @Given("^the (.*) GP appointment system is unavailable$")
    fun theAppointmentSystemIsUnavailable(gpSystem: String) {
        val currentViewAppointmentFactory = UpcomingAppointmentsFactory.getForSupplier(gpSystem)
        currentViewAppointmentFactory.createUpcomingAppointments {
            respondWithGPServiceUnavailableException()
        }
    }

    @Then("^I see page header indicating there is an appointment data error$")
    fun iSeePageHeaderIndicatingAppointmentDataError() {
        myAppointmentsSteps.verifyAppointmentDataErrorHeaderIsDisplayed()
    }

    @Then("^I see the appropriate error messages for the appointment data error$")
    fun iSeeTheAppropriateErrorMessagesForTheAppointmentDataError() {
        myAppointmentsSteps.checkAppointmentDataErrorMessagesAreCorrect()
    }
}
