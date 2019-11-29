package features.authentication.stepDefinitions

import constants.Supplier
import cucumber.api.java.en.Given
import cucumber.api.java.en.Then
import cucumber.api.java.en.When
import features.authentication.factories.Im1ConnectionV2Factory
import mocking.GsonFactory
import net.serenitybdd.core.Serenity
import org.apache.http.HttpStatus
import org.junit.Assert
import utils.SerenityHelpers
import utils.getOrFail
import utils.set
import worker.NhsoHttpException
import worker.WorkerClient
import worker.models.patient.Im1ConnectionRequest
import worker.models.patient.Im1ConnectionResponse
import worker.models.patient.Im1ConnectionToken

class Im1ConnectionV2StepDefinitions {

    @Given("^I am a (.*) user wishing to register with full linkage details$")
    fun iAmAUserWishingToRegisterWithFullLinkageDetails(gpSystem: String) {
        val supplier = Supplier.valueOf(gpSystem)
        val factory = Im1ConnectionV2Factory.getForSupplier(supplier)
        SerenityHelpers.setPatient(factory.patient)
        val connectionRequest = factory.validIm1Request
        Im1ConnectionSerenityHelpers.Im1ConnectionRequest.set(connectionRequest)
        val linkage = factory.validLinkageDetails
        factory.successfulIm1Register(linkage)
    }

    @Given("^I am a (.*) user wishing to register with retrieved linkage details$")
    fun iAmAUserWishingToRegisterWithRetrievedLinkageDetails(gpSystem: String) {
        val supplier = Supplier.valueOf(gpSystem)
        val factory = Im1ConnectionV2Factory.getForSupplier(supplier)
        SerenityHelpers.setPatient(factory.patient)
        val connectionRequest =factory.validIm1Request
        connectionRequest.AccountId = null
        val linkage = factory.validLinkageDetails
        factory.successfulLinkageGet(linkage)
        factory.successfulIm1Register(linkage)
        Im1ConnectionSerenityHelpers.Im1ConnectionRequest.set(connectionRequest)
    }


    @Given("^I am a (.*) user wishing to register with created linkage details$")
    fun iAmAUserWishingToRegisterWithCreatedLinkageDetails(gpSystem: String) {
        val supplier = Supplier.valueOf(gpSystem)
        val factory = Im1ConnectionV2Factory.getForSupplier(supplier)
        SerenityHelpers.setPatient(factory.patient)
        val connectionRequest = factory.validIm1Request
        val linkage = factory.validLinkageDetails
        factory.linkageGet(linkage) {x -> x.respondWithError(HttpStatus.SC_NOT_FOUND, "0")}
        factory.successfulLinkagePost(linkage)
        factory.successfulIm1Register(linkage)
        Im1ConnectionSerenityHelpers.Im1ConnectionRequest.set(connectionRequest)
    }

    @When("^I register the user's IM1 credentials using the v2 endpoint$")
    fun iRegisterAUsersIMCredentialsUsingV2Endpoint() {

        val request = Im1ConnectionSerenityHelpers.Im1ConnectionRequest.getOrFail<Im1ConnectionRequest>();
        try {
            val response = Serenity.sessionVariableCalled<WorkerClient>(WorkerClient::class)
                    .authentication
                    .postIm1ConnectionV2(request)
            Im1ConnectionSerenityHelpers.Im1ConnectionResponse.set(response)
        } catch (httpException: NhsoHttpException) {
            SerenityHelpers.setHttpException(httpException)
        }
    }

    @When("I verify patient data using the v2 endpoint")
    fun whenIVerifyPatientData() {
        val connectionToken = PatientVerificationSerenityHelpers.ConnectionToken.getOrFail<String>()
        val odsCode = PatientVerificationSerenityHelpers.NationalPracticeCode.getOrFail<String>()

        try {
            val result = Serenity.sessionVariableCalled<WorkerClient>(WorkerClient::class)
                    .authentication.getIm1ConnectionV2(connectionToken, odsCode)
            Serenity.setSessionVariable(Im1ConnectionResponse::class).to(result)
        } catch (httpException: NhsoHttpException) {
            SerenityHelpers.setHttpException(httpException)
        }
    }

    @When("I verify patient data without sending the ODS Code using the v2 endpoint")
    fun whenIVerifyPatientDataWithoutSendingTheOdsCode() {
        val connectionToken = PatientVerificationSerenityHelpers.ConnectionToken.getOrFail<String>()

        try {
            val result = Serenity.sessionVariableCalled<WorkerClient>(WorkerClient::class)
                    .authentication.getIm1ConnectionV2(connectionToken, null)
            Serenity.setSessionVariable(Im1ConnectionResponse::class).to(result)
        } catch (httpException: NhsoHttpException) {
            SerenityHelpers.setHttpException(httpException)
        }
    }

    @Then("^the Im1 connection response has the expected connection token$")
    fun theResponseHasTheExpectedConnectionToken() {
        val result = Im1ConnectionSerenityHelpers.Im1ConnectionResponse.getOrFail<Im1ConnectionResponse>()
        val expectedIm1ConnectionToken = SerenityHelpers.getPatient().im1ConnectionToken
        val actualIm1ConnectionToken = GsonFactory.asPascal.fromJson<Im1ConnectionToken>(
                result.connectionToken,
                Im1ConnectionToken::class.java
        )
        Assert.assertEquals(expectedIm1ConnectionToken, actualIm1ConnectionToken)
    }

    @Then("^the Im1 connection response has the expected NHS numbers")
    fun theResponseHasTheExpectedNhsNumbers() {
        val result = Im1ConnectionSerenityHelpers.Im1ConnectionResponse.getOrFail<Im1ConnectionResponse>()
        val expectedNhsNumbers = SerenityHelpers.getPatient().nhsNumbers
        val responseNhsNumbers = result.nhsNumbers!!.map { it.nhsNumber.replace(" ", "") }
        Assert.assertEquals(expectedNhsNumbers, responseNhsNumbers)
    }
}