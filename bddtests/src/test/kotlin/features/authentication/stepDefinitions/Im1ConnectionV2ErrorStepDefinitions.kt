package features.authentication.stepDefinitions

import com.google.gson.GsonBuilder
import constants.Supplier
import io.cucumber.java.en.Given
import io.cucumber.java.en.Then
import features.authentication.factories.Im1ConnectionV2Factory
import features.authentication.factories.Im1ConnectionV2GetFactory
import models.Patient
import org.apache.http.HttpStatus
import org.joda.time.DateTime
import org.junit.Assert
import utils.SerenityHelpers
import utils.set
import worker.models.patient.Im1ConnectionRequest
import worker.models.patient.Im1ErrorResponse
import java.time.Duration

private const val MINIMUM_AGE : Int = 16
private const val TIMEOUT_IN_SECONDS = 90L
class Im1ConnectionV2ErrorStepDefinitions {

    @Given("^I am a (.*) user registering with created linkage after a get linkage returns '(.*)' '(.*)' '(.*)'")
    fun iAmAUserWishingToRegisterWithCreatedLinkageDetailsAfterGetLinkageReturns(gpSystem: String,
                                                                                 gpHttpCode:Int,
                                                                                 gpError:String,
                                                                                 message: String) {
        val supplier = Supplier.valueOf(gpSystem)
        val factory = Im1ConnectionV2Factory.getForSupplier(supplier)
        SerenityHelpers.setPatient(factory.patient)
        val connectionRequest = factory.validCreateLinkageRequest
        val linkage = factory.validLinkageDetails
        factory.linkageGet(linkage) {x -> x.respondWithError(gpHttpCode, gpError, message)}
        factory.successfulLinkagePost(linkage)
        factory.successfulIm1Register(linkage)
        Im1ConnectionSerenityHelpers.Im1ConnectionRequest.set(connectionRequest)
    }

    @Given("^I am a (.*) user wishing to register but the request will timeout$")
    fun iAmAUserWishingToRegisterButTheRequestWillTimeOut(gpSystem: String) {
        val supplier = Supplier.valueOf(gpSystem)
        val factory = Im1ConnectionV2Factory.getForSupplier(supplier)
        SerenityHelpers.setPatient(factory.patient)
        val connectionRequest = factory.validIm1Request
        val linkage = factory.validLinkageDetails
        factory.linkageGet(linkage) { x -> x.respondWithError(HttpStatus.SC_NOT_FOUND, "1") }
        factory.successfulLinkagePost(linkage)
        factory.successfulIm1Register(linkage, Duration.ofSeconds(TIMEOUT_IN_SECONDS))
        Im1ConnectionSerenityHelpers.Im1ConnectionRequest.set(connectionRequest)
    }

    @Given("^I am a (.*) user wishing to register but missing Odscode$")
    fun iAmAUserWishingToRegisterWithMissingOdsCode(gpSystem: String) {
        val supplier = Supplier.valueOf(gpSystem)
        val patient = Patient.getDefault(supplier)
        val connectionRequest = Im1ConnectionRequest(
                AccountId = patient.accountId,
                LinkageKey = patient.linkageKey,
                OdsCode = null,
                Surname = patient.name.surname,
                DateOfBirth = patient.age.dateOfBirth)
        SerenityHelpers.setPatient(patient)
        Im1ConnectionSerenityHelpers.Im1ConnectionRequest.set(connectionRequest)
    }

    @Given("^I am a (.*) user wishing to register but missing Dob$")
    fun iAmAUserWishingToRegisterWithMissingDOB(gpSystem: String) {
        val supplier = Supplier.valueOf(gpSystem)
        val patient = Patient.getDefault(supplier)
        val connectionRequest = Im1ConnectionRequest(
                AccountId = patient.accountId,
                LinkageKey = patient.linkageKey,
                OdsCode = patient.odsCode,
                Surname = patient.name.surname,
                DateOfBirth = null)
        SerenityHelpers.setPatient(patient)
        Im1ConnectionSerenityHelpers.Im1ConnectionRequest.set(connectionRequest)
    }

    @Given("^I am a (.*) user wishing to register but missing Surname$")
    fun iAmAUserWishingToRegisterWithMissingSurname(gpSystem: String) {
        val supplier = Supplier.valueOf(gpSystem)
        val patient = Patient.getDefault(supplier)
        val connectionRequest = Im1ConnectionRequest(
                AccountId = patient.accountId,
                LinkageKey = patient.linkageKey,
                OdsCode = patient.odsCode,
                Surname = null,
                DateOfBirth = patient.age.dateOfBirth)
        SerenityHelpers.setPatient(patient)
        Im1ConnectionSerenityHelpers.Im1ConnectionRequest.set(connectionRequest)
    }

    @Given("^I am a (.*) user registering but getting linkage details returns '(.*)' '(.*)' '(.*)'$")
    fun iAmAUserWishingToRegisterButRegisteringWillReturnError(gpSystem: String,
                                                               gpHttpCode:Int,
                                                               gpError:String,
                                                               message: String) {
        val supplier = Supplier.valueOf(gpSystem)
        val factory = Im1ConnectionV2Factory.getForSupplier(supplier)
        SerenityHelpers.setPatient(factory.patient)
        val connectionRequest = factory.validCreateLinkageRequest
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
        val supplier = Supplier.valueOf(gpSystem)
        val factory = Im1ConnectionV2Factory.getForSupplier(supplier)
        val connectionRequest = factory.validCreateLinkageRequest
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
        val supplier = Supplier.valueOf(gpSystem)
        val factory = Im1ConnectionV2Factory.getForSupplier(supplier)
        val connectionRequest = factory.validCreateLinkageRequest
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
        val supplier = Supplier.valueOf(gpSystem)
        val factory = Im1ConnectionV2Factory.getForSupplier(supplier)
        SerenityHelpers.setPatient(factory.patient)
        val connectionRequest = factory.validIm1Request
        Im1ConnectionSerenityHelpers.Im1ConnectionRequest.set(connectionRequest)
        factory.errorIm1Register(gpHttpCode, gpError, message)
    }

    @Given("^I am a (.*) user and verifying my im1 connection returns '(.*)' '(.*)' '(.*)'$")
    fun iAmAUserWithProvidedLinkageKeyButVerifyingWillReturnError(gpSystem: String,
                                                                    gpHttpCode:Int,
                                                                    gpError:String,
                                                                    message:String) {
        val supplier = Supplier.valueOf(gpSystem)
        val im1ConnectionV2GetFactory = Im1ConnectionV2GetFactory.getForSupplier(supplier)
        SerenityHelpers.setPatient(im1ConnectionV2GetFactory.patient)
        im1ConnectionV2GetFactory.errorIm1Verify(gpHttpCode, gpError, message)
    }

    @Given("^I am a (.*) user registering but creating my linkage key will return a '(.*)' '(.*)' error$")
    fun iAmAUserWishingToRegisterButCreatingLinkageKeyWillReturnError(gpSystem: String,
                                                                      gpHttpCode:Int,
                                                                      gpError:String) {
        val supplier = Supplier.valueOf(gpSystem)
        val factory = Im1ConnectionV2Factory.getForSupplier(supplier)
        val connectionRequest = factory.validCreateLinkageRequest
        val linkage = factory.validLinkageDetails

        Im1ConnectionSerenityHelpers.Im1ConnectionRequest.set(connectionRequest)

        factory.linkageGet(linkage) {x -> x.respondWithError(HttpStatus.SC_NOT_FOUND, "0")}
        factory.linkagePost(linkage) {x -> x.respondWithError(gpHttpCode, gpError)}
        Im1ConnectionSerenityHelpers.LinkageFacade.set(linkage)
    }

    @Given("^I am a (.*) user registering but creating my linkage key fail because I am under minimum age$")
    fun iAmAUserWishingToRegisterButCreatingLinkageKeyWillFailBecauseIAmUnderMinimumAge(gpSystem: String) {
        val supplier = Supplier.valueOf(gpSystem)
        val factory = Im1ConnectionV2Factory.getForSupplier(supplier)
        val linkageDateOfBirthFormat = Im1ConnectionV2Factory.getForSupplier(supplier).linkageDateOfBirthFormat
        val dob = DateTime.now().minusYears(MINIMUM_AGE).plusDays(1).toString(linkageDateOfBirthFormat)
        val connectionRequest = factory.validCreateLinkageRequest
        connectionRequest.DateOfBirth = dob
        val linkage = factory.validLinkageDetails
        linkage.dateOfBirth = dob

        factory.linkageGet(linkage) {x -> x.respondWithError(HttpStatus.SC_NOT_FOUND, "0")}
        Im1ConnectionSerenityHelpers.Im1ConnectionRequest.set(connectionRequest)

        factory.linkageGet(linkage) {x -> x.respondWithError(HttpStatus.SC_NOT_FOUND, "0")}
        Im1ConnectionSerenityHelpers.LinkageFacade.set(linkage)
    }

    @Then("^I receive a '(\\w+)' IM1 error status code with code '(\\w+)'$")
    fun thenIReceiveAStatusCodeWithCode(expectedStatusCode: Int, expectedErrorCode:String) {
        val gpSystem = SerenityHelpers.getGpSupplier()
        assertIm1ErrorResponse(expectedStatusCode,expectedErrorCode,  gpSystem.toString())
    }

    @Then("^I receive a '(\\w+)' IM1 error status code with code '(\\w+)' and GP System 'Unknown'$")
    fun thenIReceiveAStatusCodeWithCodeAndGPSystemUnknown(expectedStatusCode: Int, expectedErrorCode:String) {
        assertIm1ErrorResponse(expectedStatusCode,expectedErrorCode,  "Unknown")
    }

    private fun assertIm1ErrorResponse(expectedStatusCode: Int, expectedErrorCode:String, gpSystem:String) {
        val errorResponse = SerenityHelpers.getHttpException()
        val errorResponseBody = errorResponse?.body.toString()
        val exception = GsonBuilder().create()
                .fromJson<Im1ErrorResponse>(errorResponseBody,
                        Im1ErrorResponse::class.java)

        Assert.assertNotNull("Expected error response", errorResponse)
        Assert.assertEquals("Expected statusCode", expectedStatusCode, errorResponse!!.statusCode)
        Assert.assertEquals("Expected errorCode", expectedErrorCode, exception?.errorCode ?: "")
        Assert.assertEquals("Expected gpSystem",
                gpSystem.toUpperCase(),
                exception?.gpSystem?.toUpperCase() ?: "")
    }
}
