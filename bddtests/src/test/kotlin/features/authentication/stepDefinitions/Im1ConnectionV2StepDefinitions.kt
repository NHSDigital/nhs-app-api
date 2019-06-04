package features.authentication.stepDefinitions

import cucumber.api.java.en.Given
import cucumber.api.java.en.Then
import cucumber.api.java.en.When
import features.authentication.factories.Im1ConnectionV2Factory
import features.sharedStepDefinitions.backend.AbstractSteps
import mocking.GsonFactory
import models.Patient
import net.serenitybdd.core.Serenity
import org.apache.http.HttpStatus
import org.joda.time.DateTime
import org.junit.Assert
import utils.SerenityHelpers
import utils.getOrFail
import utils.set
import worker.NhsoHttpException
import worker.WorkerClient
import worker.models.patient.Im1ConnectionRequest
import worker.models.patient.Im1ConnectionResponse
import worker.models.patient.Im1ConnectionToken

const val MINIMUM_AGE : Int = 16
class Im1ConnectionV2StepDefinitions : AbstractSteps() {

    @Given("^I am a (.*) user wishing to register with full linkage details$")
    fun iAmAUserWishingToRegisterWithFullLinkageDetails(gpSystem: String) {
        val factory = Im1ConnectionV2Factory.getForSupplier(gpSystem)
        SerenityHelpers.setPatient(factory.patient)
        val connectionRequest = factory.validIm1Request
        Im1ConnectionSerenityHelpers.Im1ConnectionRequest.set(connectionRequest)
        val linkage = factory.validLinkageDetails
        factory.successfulIm1Register(linkage)
    }

    @Given("^I am a (.*) user wishing to register with retrieved linkage details$")
    fun iAmAUserWishingToRegisterWithRetrievedLinkageDetails(gpSystem: String) {
        val factory = Im1ConnectionV2Factory.getForSupplier(gpSystem)
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
        val factory = Im1ConnectionV2Factory.getForSupplier(gpSystem)
        SerenityHelpers.setPatient(factory.patient)
        val connectionRequest = factory.validIm1Request
        val linkage = factory.validLinkageDetails
        factory.linkageGet(linkage) {x -> x.respondWithError(HttpStatus.SC_NOT_FOUND, "0")}
        factory.successfulLinkagePost(linkage)
        factory.successfulIm1Register(linkage)
        Im1ConnectionSerenityHelpers.Im1ConnectionRequest.set(connectionRequest)
    }

    @Given("^I am a (.*) user registering with created linkage after a get linkage returns '(.*)' '(.*)' '(.*)'")
    fun iAmAUserWishingToRegisterWithCreatedLinkageDetailsAfterGetLinkageReturns(gpSystem: String,
                                                                                 gpHttpCode:Int,
                                                                                 gpError:String,
                                                                                 message: String) {
        val factory = Im1ConnectionV2Factory.getForSupplier(gpSystem)
        SerenityHelpers.setPatient(factory.patient)
        val connectionRequest = factory.validIm1Request
        val linkage = factory.validLinkageDetails
        factory.linkageGet(linkage) {x -> x.respondWithError(gpHttpCode, gpError, message)}
        factory.successfulLinkagePost(linkage)
        factory.successfulIm1Register(linkage)
        Im1ConnectionSerenityHelpers.Im1ConnectionRequest.set(connectionRequest)
    }

    @Given("^I am a (.*) user wishing to register but missing Odscode$")
    fun iAmAUserWishingToRegisterWithMissingOdsCode(gpSystem: String) {
        val patient = Patient.getDefault(gpSystem)
        val connectionRequest = Im1ConnectionRequest(
                AccountId = patient.accountId,
                LinkageKey = patient.linkageKey,
                OdsCode = null,
                Surname = patient.surname,
                DateOfBirth = patient.dateOfBirth)
        SerenityHelpers.setPatient(patient)
        Im1ConnectionSerenityHelpers.Im1ConnectionRequest.set(connectionRequest)
    }

    @Given("^I am a (.*) user wishing to register but missing Dob$")
    fun iAmAUserWishingToRegisterWithMissingDOB(gpSystem: String) {
        val patient = Patient.getDefault(gpSystem)
        val connectionRequest = Im1ConnectionRequest(
                AccountId = patient.accountId,
                LinkageKey = patient.linkageKey,
                OdsCode = patient.odsCode,
                Surname = patient.surname,
                DateOfBirth = null)
        SerenityHelpers.setPatient(patient)
        Im1ConnectionSerenityHelpers.Im1ConnectionRequest.set(connectionRequest)
    }

    @Given("^I am a (.*) user wishing to register but missing Surname$")
    fun iAmAUserWishingToRegisterWithMissingSurname(gpSystem: String) {
        val patient = Patient.getDefault(gpSystem)
        val connectionRequest = Im1ConnectionRequest(
                AccountId = patient.accountId,
                LinkageKey = patient.linkageKey,
                OdsCode = patient.odsCode,
                Surname = null,
                DateOfBirth = patient.dateOfBirth)
        SerenityHelpers.setPatient(patient)
        Im1ConnectionSerenityHelpers.Im1ConnectionRequest.set(connectionRequest)
    }

    @Given("^I am a (.*) user registering but getting my linkage key will return a '(.*)' '(.*)' error$")
    fun iAmAUserWishingToRegisterButRetrievingLinkageKeyWillReturnError(gpSystem: String,
                                                                        gpHttpCode:Int,
                                                                        gpError:String) {
        val factory = Im1ConnectionV2Factory.getForSupplier(gpSystem)
        val connectionRequest = factory.validIm1Request
        connectionRequest.AccountId = null
        val linkage = factory.validLinkageDetails

        Im1ConnectionSerenityHelpers.Im1ConnectionRequest.set(connectionRequest)

        factory.linkageGet(linkage) {x -> x.respondWithError(gpHttpCode, gpError)}
        Im1ConnectionSerenityHelpers.LinkageFacade.set(linkage)
    }

    @Given("^I am a (.*) user registering but getting linkage details returns '(.*)' '(.*)' '(.*)'$")
    fun iAmAUserWishingToRegisterButRegisteringWillReturnError(gpSystem: String,
                                                               gpHttpCode:Int,
                                                               gpError:String,
                                                               message: String) {
        val factory = Im1ConnectionV2Factory.getForSupplier(gpSystem)
        val connectionRequest = factory.validIm1Request
        connectionRequest.AccountId = null
        val linkage = factory.validLinkageDetails

        Im1ConnectionSerenityHelpers.Im1ConnectionRequest.set(connectionRequest)

        factory.linkageGet(linkage) {x -> x.respondWithError(gpHttpCode, gpError, message)}
        Im1ConnectionSerenityHelpers.LinkageFacade.set(linkage)
    }

    @Given("^I am a (.*) user with retrieved linkage but registering returns '(.*)' '(.*)' '(.*)'$")
    fun iAmAUserWithRetreivedLinkageKeyButRegisteringWillReturnError(gpSystem: String,
                                                                     gpHttpCode:Int,
                                                                     gpError:String,
                                                                     gpErrorMessage:String) {
        val factory = Im1ConnectionV2Factory.getForSupplier(gpSystem)
        val connectionRequest = factory.validIm1Request
        connectionRequest.AccountId = null
        val linkage = factory.validLinkageDetails

        Im1ConnectionSerenityHelpers.Im1ConnectionRequest.set(connectionRequest)

        factory.successfulLinkageGet(linkage)
        factory.errorIm1Register(gpHttpCode, gpError, gpErrorMessage)
        Im1ConnectionSerenityHelpers.LinkageFacade.set(linkage)
    }

    @Given("^I am a (.*) user with created linkage key but registering returns '(.*)' '(.*)' '(.*)'$")
    fun iAmAUserWithCreatedLinkageKeyButRegisteringWillReturnError(gpSystem: String,
                                                                    gpHttpCode:Int,
                                                                    gpError:String,
                                                                    message:String) {
        val factory = Im1ConnectionV2Factory.getForSupplier(gpSystem)
        val connectionRequest = factory.validIm1Request
        connectionRequest.AccountId = null
        val linkage = factory.validLinkageDetails

        Im1ConnectionSerenityHelpers.Im1ConnectionRequest.set(connectionRequest)

        factory.linkageGet(linkage) {x -> x.respondWithError(HttpStatus.SC_NOT_FOUND, "0")}
        factory.successfulLinkagePost(linkage)
        factory.errorIm1Register(gpHttpCode, gpError, message)
        Im1ConnectionSerenityHelpers.LinkageFacade.set(linkage)
    }

    @Given("^I am a (.*) user with provided linkage key but registering returns '(.*)' '(.*)' '(.*)'$")
    fun iAmAUserWithProvidedLinkageKeyButRegisteringWillReturnError(gpSystem: String,
                                                                    gpHttpCode:Int,
                                                                    gpError:String,
                                                                    message:String) {
        val factory = Im1ConnectionV2Factory.getForSupplier(gpSystem)
        SerenityHelpers.setPatient(factory.patient)
        val connectionRequest = factory.validIm1Request
        Im1ConnectionSerenityHelpers.Im1ConnectionRequest.set(connectionRequest)
        factory.errorIm1Register(gpHttpCode, gpError, message)
    }

    @Given("^I am a (.*) user registering but creating my linkage key will return a '(.*)' '(.*)' error$")
    fun iAmAUserWishingToRegisterButCreatingLinkageKeyWillReturnError(gpSystem: String,
                                                                      gpHttpCode:Int,
                                                                      gpError:String) {
        val factory = Im1ConnectionV2Factory.getForSupplier(gpSystem)
        val connectionRequest = factory.validIm1Request
        connectionRequest.AccountId = null
        val linkage = factory.validLinkageDetails

        Im1ConnectionSerenityHelpers.Im1ConnectionRequest.set(connectionRequest)

        factory.linkageGet(linkage) {x -> x.respondWithError(HttpStatus.SC_NOT_FOUND, "0")}
        factory.linkagePost(linkage) {x -> x.respondWithError(gpHttpCode, gpError)}
        Im1ConnectionSerenityHelpers.LinkageFacade.set(linkage)
    }

    @Given("^I am a (.*) user registering but creating my linkage key fail because I am under minimum age$")
    fun iAmAUserWishingToRegisterButCreatingLinkageKeyWillFailBecauseIAmUnderMinimumAge(gpSystem: String) {
        val factory = Im1ConnectionV2Factory.getForSupplier(gpSystem)
        val linkageDateOfBirthFormat = Im1ConnectionV2Factory.getForSupplier(gpSystem).linkageDateOfBirthFormat
        val dob = DateTime.now().minusYears(MINIMUM_AGE).plusDays(1).toString(linkageDateOfBirthFormat)
        val connectionRequest = factory.validIm1Request
        connectionRequest.AccountId = null
        connectionRequest.DateOfBirth = dob
        val linkage = factory.validLinkageDetails
        linkage.dateOfBirth = dob

        factory.linkageGet(linkage) {x -> x.respondWithError(HttpStatus.SC_NOT_FOUND, "0")}
        Im1ConnectionSerenityHelpers.Im1ConnectionRequest.set(connectionRequest)

        factory.linkageGet(linkage) {x -> x.respondWithError(HttpStatus.SC_NOT_FOUND, "0")}
        Im1ConnectionSerenityHelpers.LinkageFacade.set(linkage)
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

    @Then("^the Im1 connection response has the expected connection token$")
    fun theResponseHasTheExpectedConnectionToken() {
        val result = Im1ConnectionSerenityHelpers.Im1ConnectionResponse.getOrFail<Im1ConnectionResponse>()
        val expectedIm1ConnectionToken = SerenityHelpers.getPatient().im1ConnectionTokenAsJson
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