package features.sharedSteps.backend

import com.google.gson.GsonBuilder
import cucumber.api.java.en.Then
import net.serenitybdd.core.Serenity.sessionVariableCalled
import org.apache.http.HttpResponse
import org.apache.http.HttpStatus
import org.junit.Assert.assertEquals
import org.junit.Assert.assertNotNull
import org.junit.Assert.assertTrue
import utils.SerenityHelpers
import worker.NhsoHttpException
import worker.NhsoHttpExceptionErrorBody
import java.lang.AssertionError

const val LINKAGE_NOT_SUPPORTED_RESPONSE_CODE = 550
const val ODS_CODE_NOT_FOUND_RESPONSE_CODE = 464
const val PATIENT_ID_NOT_FOUND_RESPONSE_CODE = 467
class ResponseStatusCodeSteps {

    @Then("^I (?:get|receive) (?:a|an) \"(.*)\" (?:error|response)$")
    fun thenIReceiveAnError(expectedStatusCode: String) {
        val converted = httpStatusCodeTransform(expectedStatusCode)
        val errorResponse = SerenityHelpers.getHttpException()
        assertNotNull(
                "An exception was expected but was not returned within the expected time limit.",
                errorResponse
        )
        assertEquals("Incorrect status code returned. ", converted, errorResponse!!.statusCode)
    }

    @Then("^I receive (?:a|an) \"(.*)\" error with service desk reference prefixed \"(.*)\"$")
    fun thenIReceiveAnErrorWithServiceDeskReferencePrefixed(expectedStatusCodeName: String,
                                                            expectedServiceDeskReferencePrefix: String) {
        val expectedStatusCode = httpStatusCodeTransform(expectedStatusCodeName)
        assertNhsoException(expectedStatusCode!!, null, expectedServiceDeskReferencePrefix);
    }

    @Then("the response contains an empty body$")
    fun theResponseBodyIsEmpty(){
        val errorResponse = SerenityHelpers.getHttpException()
        assertNotNull("Expected Response", errorResponse)
        assertEquals("Expected an empty body. ", "", errorResponse!!.body)
    }

    @Then("^I receive (?:a|an) \"(.*)\" success code")
    fun thenIReceiveASuccessMessage(expectedStatusCode: String) {
        val converted = httpStatusCodeTransform(expectedStatusCode)
        val httpResponse = sessionVariableCalled<HttpResponse>("HttpResponse")
        assertEquals(converted, httpResponse.statusLine.statusCode)
    }

    @Then("^I receive (?:a|an) \"(.*)\" code")
    fun thenIReceiveAMessage(expectedStatusCode: String) {
        val converted = httpStatusCodeTransform(expectedStatusCode)
        val httpResponse = sessionVariableCalled<HttpResponse>("HttpResponse")
        assertEquals(converted, httpResponse.statusLine.statusCode)
    }

    @Then("^I receive (?:a|an) \"(.*)\" error status code$")
    fun thenIReceiveAStatusCode(expectedStatusCode: Int) {
        val exception = sessionVariableCalled<NhsoHttpException>("HttpException")
        assertEquals(expectedStatusCode, exception.statusCode)
    }

    @Then("^I receive (?:a|an) \"(.*)\" error status code with service desk reference prefixed \"(.*)\"$")
    fun thenIReceiveAStatusCodeWithServiceDeskReferencePrefixed(expectedStatusCode: Int,
                                                                expectedServiceDeskReferencePrefix: String) {
        assertNhsoException(expectedStatusCode, null, expectedServiceDeskReferencePrefix)
    }

    @Then("^I receive (?:a|an) \"(.*)\" error status code with code \"(.*)\"$")
    fun thenIReceiveAStatusCodeWithCode(expectedStatusCode: Int, errorCode:String) {
        assertNhsoException(expectedStatusCode, errorCode)
    }

    @Then("the Bad Gateway error response includes a retry option$")
    fun theOrganDonationBadGatewayErrorResponseDoesIncludeARetryOption(){
        assertNhsoException( HttpStatus.SC_BAD_GATEWAY, "1")
    }

    @Then("the Bad Gateway error response does not include a retry option$")
    fun theOrganDonationBadGatewayErrorResponseDoesNotIncludeARetryOption(){
        assertNhsoException( HttpStatus.SC_BAD_GATEWAY, "0")
    }

    @Then("the Internal Server Error response does not include a retry option$")
    fun theOrganDonationInternalServerErrorResponseDoesNotIncludeARetryOption(){
        val errorResponse = SerenityHelpers.getHttpException()
        assertNotNull("Expected Response", errorResponse)
        assertEquals("Expected Body", "", errorResponse?.body.toString())
        assertEquals("Expected statusCode", HttpStatus.SC_INTERNAL_SERVER_ERROR, errorResponse!!.statusCode)
    }

    private fun assertNhsoException(expectedHttpStatusCode: Int,
                                    expectedErrorCode: String? = null,
                                    expectedServiceDeskReferencePrefix: String? = null) {
        val errorResponse = SerenityHelpers.getHttpException()

        val exception = try {
            GsonBuilder().create()
                    .fromJson(errorResponse?.body.toString(),
                            NhsoHttpExceptionErrorBody::class.java)
        } catch (ex: java.lang.Exception) {
            throw AssertionError("Failed to deserialise JSON error response of:\n${errorResponse?.body}")
        }

        assertNotNull("Expected Response", errorResponse)
        assertEquals("Expected statusCode", expectedHttpStatusCode, errorResponse!!.statusCode)

        if (expectedErrorCode!=null){
            assertEquals("Expected errorCode", expectedErrorCode, exception?.errorCode ?: "")
        }

        if (expectedServiceDeskReferencePrefix!=null){
            assertTrue(
                    "Expected service desk error reference to start with " +
                            expectedServiceDeskReferencePrefix + "  but was " +
                            exception?.serviceDeskReference,
                    (exception?.serviceDeskReference ?: "").startsWith(expectedServiceDeskReferencePrefix))
        }
    }

    private val _statusCodeMapping: HashMap<String, Int> = hashMapOf(
            "ok" to HttpStatus.SC_OK,
            "created" to HttpStatus.SC_CREATED,
            "bad gateway" to HttpStatus.SC_BAD_GATEWAY,
            "bad request" to HttpStatus.SC_BAD_REQUEST,
            "gateway timeout" to HttpStatus.SC_GATEWAY_TIMEOUT,
            "not found" to HttpStatus.SC_NOT_FOUND,
            "internal server error" to HttpStatus.SC_INTERNAL_SERVER_ERROR,
            "conflict" to HttpStatus.SC_CONFLICT,
            "forbidden" to HttpStatus.SC_FORBIDDEN,
            "service unavailable" to HttpStatus.SC_SERVICE_UNAVAILABLE,
            "unauthorized" to HttpStatus.SC_UNAUTHORIZED,
            "not implemented" to HttpStatus.SC_NOT_IMPLEMENTED,
            "linkage not supported" to LINKAGE_NOT_SUPPORTED_RESPONSE_CODE,
            "no content" to HttpStatus.SC_NO_CONTENT,
            "ods code not found" to ODS_CODE_NOT_FOUND_RESPONSE_CODE,
            "patient id not found" to PATIENT_ID_NOT_FOUND_RESPONSE_CODE
    )

    private fun httpStatusCodeTransform(statusName: String): Int? {
        return _statusCodeMapping[statusName.toLowerCase()]
                ?: throw IllegalArgumentException("Could not identify an HTTP status code named: $statusName")
    }
}
