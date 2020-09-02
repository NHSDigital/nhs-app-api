package features.im1Appointments.stepDefinitions

import constants.Supplier
import io.cucumber.java.en.Given
import io.cucumber.java.en.Then
import features.im1Appointments.steps.CancelAppointmentSteps
import mocking.stubs.StubbedEnvironment
import mockingFacade.appointments.MyAppointmentsFacade
import net.serenitybdd.core.Serenity
import net.thucydides.core.annotations.Steps
import pages.ErrorDialogPage
import java.time.Duration

class CancelAppointmentStepDefinitions {

    @Steps
    private lateinit var cancelAppointmentSteps: CancelAppointmentSteps
    private lateinit var errorDialogPage: ErrorDialogPage

    @Given("^(.*) is available to cancel a previously booked appointment before cutoff time because (.*)$")
    fun gpSystemIsAvailableToCancelAnAppointmentForReason(gpSystem: String, reason: String) {
        val supplier = Supplier.valueOf(gpSystem)
        cancelAppointmentSteps.mockCancellationRequestStubForReason(reason, supplier) { cancelRequest ->
            cancelRequest.respondWithSuccess()
        }
    }

    @Given("^VISION is available to cancel a previously booked appointment before cutoff time, " +
            "with only one available reason$")
    fun visionIsAvailableToCancelWithOneReason() {
        cancelAppointmentSteps.mockCancellationRequestStubForReason(gpSystem = Supplier.VISION) { cancelRequest ->
            cancelRequest.respondWithSuccess()
        }
    }

    @Given("^I select a cancellation reason of (.*)$")
    fun iSelectACancellationReason(reason: String) {
        cancelAppointmentSteps.selectReason(reason)
    }

    @Given("^I select the cancellation reason$")
    fun iSelectTheCancellationReason() {
        iSelectACancellationReason(
                Serenity.sessionVariableCalled<MyAppointmentsFacade>(MyAppointmentsFacade::class)
                        .myAppointments!!
                        .cancellationReasons!!
                        .first()
                        .displayName
        )
    }

    @Given("^(.*) user is not allowed to cancel appointments with '(.*)'$")
    fun gpSystemUserIsNotAllowedToCancelAppointmentsWithReason(gpSystem: String, reason: String) {
        val supplier = Supplier.valueOf(gpSystem)
        cancelAppointmentSteps.mockCancellationRequestStubForReason(reason, supplier) { cancelRequest ->
            cancelRequest.responseErrorForbiddenService()
        }
    }

    @Given("^(.*) prevents cancellation of previously booked appointment with '(.*)' because it is already cancelled$")
    fun gpSystemPreventsCancellationWithReasonBecauseItIsAlreadyCancelled(gpSystem: String, reason: String) {
        val supplier = Supplier.valueOf(gpSystem)
        cancelAppointmentSteps.mockCancellationRequestStubForReason(reason, supplier) { cancelRequest ->
            cancelRequest.respondWithExceptionWhenNotAvailable()
        }
    }

    @Given("^TPP prevents cancellation of previously booked appointment because it is too late$")
    fun tppPreventsCancellationWithReasonBecauseItIsTooLate() {
        cancelAppointmentSteps.mockCancellationRequestStubForReason("", Supplier.TPP) { cancelRequest ->
            cancelRequest.respondWithWithinAnHour()
        }
    }

    @Given("^VISION returns corrupt data when cancelling appointment with '(.*)'$")
    fun visionReturnsCorruptDataWhenCancellingAppointmentWithReason(reason: String) {
        cancelAppointmentSteps.mockCancellationRequestStubForReason(reason, Supplier.VISION) { cancelRequest ->
            cancelRequest.respondWithCorrupted()
        }
    }

    @Given("^EMIS returns unknown exception when cancelling appointment with '(.*)'$")
    fun emisReturnsUnknownExceptionaWhenCancellingAppointmentWithReason(reason: String) {
        cancelAppointmentSteps.mockCancellationRequestStubForReason(reason, Supplier.EMIS) { cancelRequest ->
            cancelRequest.respondWithUnknownException()
        }
    }

    @Given("^(.*) will time out when trying to cancel with '(.*)'$")
    fun gpSystemWillTimeOutWhenTryingToCancelWithReason(gpSystem: String, reason: String) {
        val supplier = Supplier.valueOf(gpSystem)
        cancelAppointmentSteps.mockCancellationRequestStubForReason(reason, supplier) {
            cancelRequest -> cancelRequest.respondWithSuccess()
                .delayedBy(Duration.ofSeconds(StubbedEnvironment.TIMEOUT_DELAY))
        }
    }

    @Then("^I see an appropriate error message when it is already cancelled$")
    fun iSeeAnAppropriateErrorMessageWhenItIsAlreadyCancelled() {
        val pageTitle = cancelAppointmentSteps.cancelAppointmentPage.cannotCancelTitle
        val pageHeader = cancelAppointmentSteps.cancelAppointmentPage.cannotCancelTitle
        val message = cancelAppointmentSteps.cancelAppointmentPage.alreadyCancelled

        errorDialogPage.assertParagraphText(message)
                .assertPageHeader(pageHeader)
                .assertPageTitle(pageTitle)
    }

    @Then("^I see an appropriate error message when it is too late to cancel$")
    fun iSeeAnAppropriateErrorMessageWhenItIsTooLateToCancel() {
        val pageTitle = cancelAppointmentSteps.cancelAppointmentPage.contactToCancelTitle
        val pageHeader = cancelAppointmentSteps.cancelAppointmentPage.contactToCancelTitle
        val message = cancelAppointmentSteps.cancelAppointmentPage.tooLateToCancel

        errorDialogPage.assertParagraphText(message)
                .assertPageHeader(pageHeader)
                .assertPageTitle(pageTitle)
    }

    @Then("^I see appropriate submit error message when there is an error with '(.*)'$")
    fun iSeeAppropriateSubmitErrorMessageWhenThereIsAnErrorWithPrefix(prefix: String) {
        val goBackParagraph = cancelAppointmentSteps.cancelAppointmentPage.getGoBackAndTryAgainParagraph(prefix)
        errorDialogPage.assertParagraphText(goBackParagraph)
                .assertParagraphText(cancelAppointmentSteps.cancelAppointmentPage.ifItContinuesBookOrCancel)
                .assertPageHeader(cancelAppointmentSteps.cancelAppointmentPage.problemHeader)
                .assertPageTitle(cancelAppointmentSteps.cancelAppointmentPage.problemTitle)
    }

    @Then("^I will be on the \"Cancellation reason\" screen$")
    fun iWillBeOnTheCancellationScreen() {
        cancelAppointmentSteps.verifyWeAreOnTheCancelAppointmentScreen()
    }

    @Then("^I am presented with the appointment details$")
    fun iAmPresentedWithTheSelectedAppointmentDetails() {
        cancelAppointmentSteps.verifyTheCorrectAppointmentDetailsAreDisplayed()
    }

    @Then("^there is a cancellation reasons drop-down$")
    fun thereIsACancellationReasonsDropDownWithTheAppropriateReasons() {
        cancelAppointmentSteps.verifyTheDropDownMenuLabel()
        cancelAppointmentSteps.selectReason("Select reason")
    }

    @Then("^cancellation reasons drop-down is hidden$")
    fun cancellationReasonsDropDownIsHidden() {
        cancelAppointmentSteps.verifyTheDropDownMenuLabelIsNotVisible()
    }

    @Then("^I will receive a cancellation validation error$")
    fun iWillReceiveACancellationValidationError() {
        cancelAppointmentSteps.verifyTheValidationErrorSummary()
        cancelAppointmentSteps.verifyTheInlineReasonValidationError()
    }
}
