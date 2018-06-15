package features.sharedStepDefinitions.backend

import cucumber.api.java.en.Given
import cucumber.api.java.en.Then
import cucumber.api.java.en.When
import mocking.emis.models.AssociationType
import worker.models.demographics.PatientIdentifier
import models.Patient
import net.serenitybdd.core.Serenity.sessionVariableCalled
import net.serenitybdd.core.Serenity.setSessionVariable

import org.apache.http.HttpStatus
import org.junit.Assert
import worker.NhsoHttpException
import worker.WorkerClient
import worker.models.patient.Im1ConnectionResponse


class PatientVerificationSteps : AbstractSteps() {

    private val defaultOdsCode = "E87649"
    private val defaultConnectionToken = "bce74b97-4296-414a-a4f5-0f1bf5732ba6"

    @Given("I have an IM1 Connection Token that does not exist")
    fun givenIHaveAnImConnectionTokenThatDoesNotExist() {
        val nonExistingConnectionToken = "0d135b66-a8b0-46b2-b437-cfe75edc773d"
        val patient = Patient(
                connectionToken = nonExistingConnectionToken,
                odsCode = defaultOdsCode,
                endUserSessionId = "zVGrzHH7YUPeEBRk1nat1D"
        )

        mockingClient.forEmis { sessionRequest(patient).respondWithUserNotRegistered() }
        mockingClient.forEmis { endUserSessionRequest().respondWithSuccess(patient.endUserSessionId) }

        setSessionVariable("ConnectionToken").to(nonExistingConnectionToken)
        setSessionVariable("NationalPracticeCode").to(patient.odsCode)
    }

    @Given("I have an IM1 Connection Token that is in an invalid format")
    fun givenIHaveAnIm1ConnectionTokenThatIsInAnInvalidFormat() {
        setSessionVariable("ConnectionToken").to("token")
        setSessionVariable("NationalPracticeCode").to(defaultOdsCode)
    }

    @Given("I have no IM1 Connection Token")
    fun givenIHaveNoIm1ConnectionToken() {
        setSessionVariable("ConnectionToken").to(null)
        setSessionVariable("NationalPracticeCode").to(defaultOdsCode)
    }

    @Given("I have an ODS Code not in expected format")
    fun givenIHaveAnOdsCodeNotInExpectedFormat() {
        setSessionVariable("ConnectionToken").to(defaultConnectionToken)
        setSessionVariable("NationalPracticeCode").to("£$*&")
    }

    @Given("I have an ODS Code that does not exists")
    fun givenIHaveAnOdsCodeThatDoesNotExists() {

        setSessionVariable("ConnectionToken").to(defaultConnectionToken)
        setSessionVariable("NationalPracticeCode").to("E99999")
    }

    @Given("I have no ODS Code")
    fun givenIHaveNoOdsCode() {
        setSessionVariable("ConnectionToken").to(defaultConnectionToken)
        setSessionVariable("NationalPracticeCode").to(null)
    }

    @Given("I have valid credentials for a patient with one NHS Number")
    fun givenIHaveValidCredentialsForAPatientWithOneNhsNumber() {
        val patient = Patient(
                title = "Mr",
                firstName = "Eduardo",
                surname = "Crouch",
                connectionToken = defaultConnectionToken,
                odsCode = defaultOdsCode,
                endUserSessionId = "zVGrzHH7YUPeEBRk1nat1D",
                sessionId = "h3pYG9By2tVTqcvPvpw3DL",
                userPatientLinkToken = "5d4p6ZExhi97mmerMrtD5p",
                nhsNumbers = listOf("NHS_number")
        )

        mockingClient.forEmis { demographicsRequest(patient).respondWithSuccess(patient, arrayOf(PatientIdentifier(patient.nhsNumbers[0]))) }
        mockingClient.forEmis { endUserSessionRequest().respondWithSuccess(patient.endUserSessionId) }
        mockingClient.forEmis { sessionRequest(patient).respondWithSuccess(patient, AssociationType.Self) }

        setSessionVariable("ConnectionToken").to(patient.connectionToken)
        setSessionVariable("NationalPracticeCode").to(patient.odsCode)
        setSessionVariable("NhsNumber").to(patient.nhsNumbers[0])
    }

    @Given("I have valid credentials for a patient with multiple NHS Numbers")
    fun givenIHaveValidCredentialsForAPatientWithMultipleNhsNumbers() {
        val patient = Patient(
                title = "Miss",
                firstName = "Alexia",
                surname = "Scott",
                odsCode = defaultOdsCode,
                connectionToken = "fe81f191-b016-466e-aeb2-64f08f2330a4",
                sessionId = "xkWiivK1WBAkxIN9CDrGyy",
                endUserSessionId = "9RFDWiqTO8zBWrp2p8s4K7",
                userPatientLinkToken = "KxLiDl5nRS60DzIlrKoFSl",
                nhsNumbers = listOf("NHS_number1", "NHS_number2")
        )
        val nhsNumbers = arrayOf(PatientIdentifier(patient.nhsNumbers[0]), PatientIdentifier(patient.nhsNumbers[1]))

        mockingClient.forEmis { endUserSessionRequest().respondWithSuccess(patient.endUserSessionId) }
        mockingClient.forEmis { sessionRequest(patient).respondWithSuccess(patient, AssociationType.Self) }
        mockingClient.forEmis { demographicsRequest(patient).respondWithSuccess(patient, nhsNumbers) }

        setSessionVariable("ConnectionToken").to(patient.connectionToken)
        setSessionVariable("NationalPracticeCode").to(patient.odsCode)
        setSessionVariable("NhsNumbers").to(nhsNumbers)
    }

    @Given("I have valid credentials for a patient with no NHS Number")
    fun givenIHaveValidCredentialsForAPatientWithNoNhsNumber() {
        val patient = Patient(
                title = "Mr",
                firstName = "Rajan",
                surname = "Liu",
                odsCode = defaultOdsCode,
                connectionToken = "e69ddbd4-2d89-43b7-a252-06dba3558f9f",
                endUserSessionId = "igOhJWsZ6GOBjaZU5PdR37",
                sessionId = "ALtNiTSBVk7VwCe1s4L1mz",
                userPatientLinkToken = "vOXLnnw7QLQoghDyqTd1Sa"
        )

        mockingClient.forEmis { endUserSessionRequest().respondWithSuccess(patient.endUserSessionId) }
        mockingClient.forEmis { sessionRequest(patient).respondWithSuccess(patient, AssociationType.Self) }
        mockingClient.forEmis { demographicsRequest(patient).respondWithSuccess(patient, arrayOf()) }

        setSessionVariable("ConnectionToken").to(patient.connectionToken)
        setSessionVariable("NationalPracticeCode").to(patient.odsCode)
    }

    @When("I verify patient data")
    fun whenIVerifyPatientData() {
        val connectionToken = sessionVariableCalled<String>("ConnectionToken")
        val odsCode = sessionVariableCalled<String>("NationalPracticeCode")


        try {
            val result = sessionVariableCalled<WorkerClient>(WorkerClient::class).getIm1Connection(connectionToken, odsCode)
            setSessionVariable(Im1ConnectionResponse::class).to(result)
        } catch (httpException: NhsoHttpException) {
            setSessionVariable("HttpException").to(httpException)
        }

    }

    @Then("I receive the expected NHS Number$")
    fun thenIReceiveTheExpectedNhsNumber() {
        val result = sessionVariableCalled<Im1ConnectionResponse>(Im1ConnectionResponse::class)
        val nhsNumber = sessionVariableCalled<String>("NhsNumber")
        val connectionToken = sessionVariableCalled<String>("ConnectionToken")

        Assert.assertNotNull(result)
        Assert.assertEquals(result.nhsNumbers!![0].nhsNumber, nhsNumber)
        Assert.assertEquals(result.nhsNumbers!![0].nhsNumber, nhsNumber)
        Assert.assertEquals(result.connectionToken, connectionToken)
    }

    @Then("I receive the expected NHS Numbers")
    fun thenIReceiveTheExpectedNhsNumbers() {
        val result = sessionVariableCalled<Im1ConnectionResponse>(Im1ConnectionResponse::class)
        val nhsNumbers = sessionVariableCalled<Array<PatientIdentifier>>("NhsNumbers")
        val connectionToken = sessionVariableCalled<String>("ConnectionToken")
        Assert.assertNotNull(result)
        Assert.assertEquals(result.nhsNumbers!!.count(), nhsNumbers.count())
        Assert.assertEquals(result.connectionToken, connectionToken)
    }

    @Then("I receive no NHS Number")
    fun thenIReceiveNoNhsNumber() {
        val result = sessionVariableCalled<Im1ConnectionResponse>(Im1ConnectionResponse::class)
        Assert.assertEquals(result.nhsNumbers!!.count(), 0)
    }

    companion object {
        val defaultOdsCode = "E87649"
        val defaultConnectionToken = "bce74b97-4296-414a-a4f5-0f1bf5732ba6"
        val getIm1ConnectionResult = "GetIm1ConnectionResult"
        private val defaultSessionId = "session_id"
        private val defaultLinkToken = "link_token"
        private val defaultEndUserSessionId = "bar"
        private val defaultAssociationType = AssociationType.Self
        private val errorMapping = mapOf("bad request" to HttpStatus.SC_BAD_REQUEST,
                "not found" to HttpStatus.SC_NOT_FOUND,
                "internal server error" to HttpStatus.SC_INTERNAL_SERVER_ERROR)
    }
}