package features.prescriptions.stepDefinitions

import constants.ErrorResponseCodeTpp
import cucumber.api.java.en.Given
import cucumber.api.java.en.Then
import mocking.MockingClient
import mocking.defaults.EmisMockDefaults
import mocking.stubs.prescriptions.factories.PrescriptionsFactory
import mocking.tpp.models.Error
import org.apache.http.HttpStatus
import org.junit.Assert
import pages.ErrorPage
import pages.navigation.WebHeader
import pages.prescription.ConfirmRepeatPrescriptionsOrderPage
import pages.prescription.ViewOrdersPrescriptionsPage
import pages.text
import utils.SerenityHelpers

class PrescriptionsErrorStepDefinitions {

    private val mockingClient = MockingClient.instance

    private lateinit var prescriptionsViewOrdersPage: ViewOrdersPrescriptionsPage
    private lateinit var confirmRepeatPrescriptionsOrderPage: ConfirmRepeatPrescriptionsOrderPage
    private lateinit var errorPage: ErrorPage
    private lateinit var webHeader: WebHeader


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

    @Given("the prescription submission endpoint is timing out")
    fun thePrescriptionSubmissionEndpointIsTimingOut() {
        val patient = SerenityHelpers.getPatient()
        val factory = PrescriptionsFactory.getForSupplier(SerenityHelpers.getGpSupplier())
        factory.prescriptionsEndpointTimeout(patient)
    }

    @Given("The prescription submission endpoint is throwing a server error")
    fun butThePrescriptionSubmissionEndpointIsThrowingAServerError() {
        mockingClient.forEmis.mock { prescriptions.repeatPrescriptionSubmissionRequest(EmisMockDefaults.patientEmis)
                .respondWith(HttpStatus.SC_INTERNAL_SERVER_ERROR, resolve = {}) }
    }

    @Given("The prescription submission endpoint is throwing an already ordered exception")
    fun butThePrescriptionSubmissionEndpointIsThrowingAnAlreadyOrderedException() {
        val patient = SerenityHelpers.getPatient()
        mockingClient.forTpp.mock {
            prescriptions.prescriptionSubmission(patient, null)
                    .respondWithError(Error(ErrorResponseCodeTpp.MEDICATION_UNAVAILABLE,
                            "One of the medications requested is no longer available",
                            "1f907c07-9063-4d3a-81d7-ee8c98c54f4a"))
        }
    }

    @Given("The prescription submission endpoint is throwing an invalid guid exception")
    fun butThePrescriptionSubmissionEndpointIsThrowingAnInvalidGuidException() {
        val patient = SerenityHelpers.getPatient()
        mockingClient.forTpp.mock { prescriptions.prescriptionSubmission(patient, null)
                .respondWithError(Error(ErrorResponseCodeTpp.MEDICATION_UNAVAILABLE,
                        "There was an error processing your request",
                        "1f907c07-9063-4d3a-81d7-ee8c98c54f4a")) }
    }

    @Then("I see the appropriate error message for a prescription timeout")
    fun thenISeeTheAppropriateErrorMessageForAPrescriptionTimeout() {

        val pageHeader = prescriptionsViewOrdersPage.timeoutPageHeader
        val header = prescriptionsViewOrdersPage.timeoutHeader
        val message = prescriptionsViewOrdersPage.timeoutMessage
        val retryButtonText = prescriptionsViewOrdersPage.timeoutRetryButtonText

        errorPage.assertHeaderText(header)
                .assertMessageText(message)
                .assertRetryButtonText(retryButtonText)
                .assertPageHeader(pageHeader)
    }

    @Then("I see the appropriate error message for a prescription server error")
    fun thenISeeTheAppropriateErrorMessageForAPrescriptionServerError() {

        val pageHeader = prescriptionsViewOrdersPage.serverErrorPageHeader
        val header = prescriptionsViewOrdersPage.serverErrorHeader
        val message = prescriptionsViewOrdersPage.serverErrorMessage

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

    @Then("I see a message informing me that I don't currently have access to this service")
    fun iSeeAMessageInformingMeThatIdontCurrentlyHaveAccessToThisService() {
        webHeader.getPageTitle().withText("Repeat prescriptions unavailable")
        Assert.assertEquals("You are not currently able to order repeat prescriptions online", errorPage.heading.text)
        Assert.assertEquals("Contact your GP surgery for more information. " +
                "For urgent medical help, call 111.", errorPage.errorText1.text)
    }
}
