package features.prescriptions.stepDefinitions

import constants.ErrorResponseCodeTpp
import cucumber.api.java.en.Given
import cucumber.api.java.en.Then
import features.prescriptions.factories.PrescriptionsFactory
import features.prescriptions.steps.PrescriptionsSteps
import mocking.MockingClient
import mocking.defaults.EmisMockDefaults
import mocking.defaults.TppMockDefaults
import mocking.tpp.models.Error
import net.thucydides.core.annotations.Steps
import org.apache.http.HttpStatus
import pages.ErrorPage
import pages.prescription.ConfirmRepeatPrescriptionsOrderPage
import pages.prescription.PrescriptionsPage
import utils.SerenityHelpers

class PrescriptionsErrorStepDefinitions {

    @Steps
    lateinit var prescriptions: PrescriptionsSteps

    val mockingClient = MockingClient.instance

    lateinit var prescriptionsPage: PrescriptionsPage
    lateinit var confirmRepeatPrescriptionsOrderPage: ConfirmRepeatPrescriptionsOrderPage
    lateinit var errorPage: ErrorPage


    @Given("The prescriptions endpoint is timing out")
    fun butThePrescriptionsEndpointIsTimingOut() {
        val factory = PrescriptionsFactory.getForSupplier(SerenityHelpers.getGpSupplier())
        factory.prescriptionsEndpointTimeout(SerenityHelpers.getPatient())
    }

    @Given("The prescriptions endpoint is throwing a server error")
    fun butThePrescriptionsEndpointIsThrowingAServerError() {
        val factory = PrescriptionsFactory.getForSupplier(SerenityHelpers.getGpSupplier())
        factory.prescriptionsEndpointThrowServerError(SerenityHelpers.getPatient())
    }

    @Given("The courses endpoint is timing out")
    fun butTheCoursesEndpointIsTimingOut() {
        val factory = PrescriptionsFactory.getForSupplier(SerenityHelpers.getGpSupplier())
        factory.coursesEndpointTimeout(SerenityHelpers.getPatient())
    }

    @Given("The courses endpoint is throwing a server error")
    fun butTheCoursesEndpointIsThrowingAServerError() {
        val factory = PrescriptionsFactory.getForSupplier(SerenityHelpers.getGpSupplier())
        factory.coursesEndpointThrowingServerError(SerenityHelpers.getPatient())
    }

    @Given("The prescription submission endpoint is timing out")
    fun butThePrescriptionSubmissionEndpointIsTimingOut() {
        mockingClient.forEmis { prescriptions.repeatPrescriptionSubmissionRequest(EmisMockDefaults.patientEmis)
                .respondWith(HttpStatus.SC_GATEWAY_TIMEOUT, resolve = {}, milliSecondDelay = 15000) }

        mockingClient.forTpp { prescriptions.prescriptionSubmission(TppMockDefaults.patientTpp, null)
                .respondWith(HttpStatus.SC_GATEWAY_TIMEOUT, resolve = {}, milliSecondDelay = 15000) }
    }

    @Given("The prescription submission endpoint is throwing a server error")
    fun butThePrescriptionSubmissionEndpointIsThrowingAServerError() {
        mockingClient.forEmis { prescriptions.repeatPrescriptionSubmissionRequest(EmisMockDefaults.patientEmis)
                .respondWith(HttpStatus.SC_INTERNAL_SERVER_ERROR, resolve = {}) }
    }

    @Given("The prescription submission endpoint is throwing an already ordered exception")
    fun butThePrescriptionSubmissionEndpointIsThrowingAnAlreadyOrderedException() {
        mockingClient.forTpp {
            prescriptions.prescriptionSubmission(TppMockDefaults.patientTpp, null)
                    .respondWithError(Error(ErrorResponseCodeTpp.MEDICATION_UNAVAILABLE,
                            "One of the medications requested is no longer available",
                            "1f907c07-9063-4d3a-81d7-ee8c98c54f4a"))
        }
    }

    @Given("The prescription submission endpoint is throwing an invalid guid exception")
    fun butThePrescriptionSubmissionEndpointIsThrowingAnInvalidGuidException() {
        mockingClient.forTpp { prescriptions.prescriptionSubmission(TppMockDefaults.patientTpp, null)
                .respondWithError(Error(ErrorResponseCodeTpp.MEDICATION_UNAVAILABLE,
                        "There was an error processing your request",
                        "1f907c07-9063-4d3a-81d7-ee8c98c54f4a")) }
    }

    @Then("I see the appropriate error message for a prescription timeout")
    fun thenISeeTheAppropriateErrorMessageForAPrescriptionTimeout() {

        val pageHeader = prescriptionsPage.timeoutPageHeader
        val header = prescriptionsPage.timeoutHeader
        val message = prescriptionsPage.timeoutMessage
        val retryButtonText = prescriptionsPage.timeoutRetryButtonText

        errorPage.assertHeaderText(header)
                .assertMessageText(message)
                .assertRetryButtonText(retryButtonText)
                .assertPageHeader(pageHeader)
    }

    @Then("I see the appropriate error message for a prescription server error")
    fun thenISeeTheAppropriateErrorMessageForAPrescriptionServerError() {

        val pageHeader = prescriptionsPage.serverErrorPageHeader
        val header = prescriptionsPage.serverErrorHeader
        val message = prescriptionsPage.serverErrorMessage

        errorPage.assertHeaderText(header)
                .assertMessageText(message)
                .assertNoRetryButton()
                .assertPageHeader(pageHeader)
    }

    @Then("I see the appropriate error message for a course request error")
    fun thenISeeTheAppropriateErrorMessageForACourseRequestError() {

        val pageHeader = confirmRepeatPrescriptionsOrderPage.serverErrorPageHeader
        val header = confirmRepeatPrescriptionsOrderPage.serverErrorHeader
        val message = confirmRepeatPrescriptionsOrderPage.serverErrorMessage
        val retryButtonText = confirmRepeatPrescriptionsOrderPage.serverErrorRetryButtonText

        errorPage.assertHeaderText(header)
                .assertMessageText(message)
                .assertRetryButtonText(retryButtonText)
                .assertPageHeader(pageHeader)
    }
}
