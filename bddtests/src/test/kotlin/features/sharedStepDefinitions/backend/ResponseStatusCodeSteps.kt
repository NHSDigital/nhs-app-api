package features.sharedStepDefinitions.backend

import com.google.gson.GsonBuilder
import cucumber.api.java.en.Then
import net.serenitybdd.core.Serenity.sessionVariableCalled
import org.apache.http.HttpResponse
import org.apache.http.HttpStatus
import org.junit.Assert
import org.junit.Assert.assertEquals
import org.junit.Assert.assertNotNull
import utils.SerenityHelpers
import worker.NhsoHttpException
import worker.NhsoHttpExceptionErrorBody

class ResponseStatusCodeSteps {

    @Then("^I receive (?:a|an) \"(.*)\" error$")
    fun thenIReceiveAMessage(expectedStatusCode: String) {
        val converted = httpStatusCodeTransform(expectedStatusCode)
        val errorResponse = SerenityHelpers.getHttpException()
        assertNotNull(
                "An exception was expected but was not returned within the expected time limit.",
                errorResponse
        )
        assertEquals("Incorrect status code returned. ", converted, errorResponse!!.statusCode)
    }

    @Then("the response contains an empty body$")
    fun theResponseBodyIsEmpty(){
        val errorResponse = SerenityHelpers.getHttpException()
        Assert.assertNotNull("Expected Response", errorResponse)
        assertEquals("Expected an empty body. ", "", errorResponse!!.body)
    }

    @Then("^I receive (?:a|an) \"(.*)\" success code")
    fun thenIReceiveASuccessMessage(expectedStatusCode: String) {
        val converted = httpStatusCodeTransform(expectedStatusCode)
        val httpResponse = sessionVariableCalled<HttpResponse>("HttpResponse")
        assertEquals(converted, httpResponse.statusLine.statusCode)
    }

    @Then("^I receive (?:a|an) \"(.*)\" error status code$")
    fun thenIReceiveAStatusCode(expectedStatusCode: Int) {
        val exception = sessionVariableCalled<NhsoHttpException>("HttpException")
        assertEquals(expectedStatusCode, exception.statusCode)
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
        Assert.assertNotNull("Expected Response", errorResponse)
        Assert.assertEquals("Expected Body", "", errorResponse?.body.toString())
        Assert.assertEquals("Expected statusCode", HttpStatus.SC_INTERNAL_SERVER_ERROR, errorResponse!!.statusCode)
    }

    private fun assertNhsoException(expectedHttpStatusCode : Int, expectedErrorCode : String){
        val errorResponse = SerenityHelpers.getHttpException()
        val errorResponseBody = GsonBuilder().create()
                .fromJson<NhsoHttpExceptionErrorBody>(errorResponse?.body.toString(),
                        NhsoHttpExceptionErrorBody::class.java)

        Assert.assertNotNull("Expected Response", errorResponse)
        Assert.assertEquals("Expected statusCode", expectedHttpStatusCode, errorResponse!!.statusCode)
        Assert.assertEquals("Expected errorCode", expectedErrorCode, errorResponseBody?.errorCode?:"")
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
            "not implemented" to HttpStatus.SC_NOT_IMPLEMENTED
    )

    private fun httpStatusCodeTransform(statusName: String): Int? {
        return _statusCodeMapping[statusName.toLowerCase()]
                ?: throw IllegalArgumentException("Could not identify an HTTP status code named: $statusName")
    }
}
