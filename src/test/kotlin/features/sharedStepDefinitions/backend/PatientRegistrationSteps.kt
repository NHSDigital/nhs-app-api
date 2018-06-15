package features.sharedStepDefinitions.backend

import cucumber.api.java.en.Given
import cucumber.api.java.en.Then
import cucumber.api.java.en.When

import mocking.emis.models.AssociationType
import mocking.emis.models.IdentifierType
import worker.models.demographics.PatientIdentifier
import models.Patient
import net.serenitybdd.core.Serenity.sessionVariableCalled
import net.serenitybdd.core.Serenity.setSessionVariable

import org.junit.Assert

import worker.NhsoHttpException
import worker.WorkerClient
import worker.models.patient.Im1ConnectionRequest
import worker.models.patient.Im1ConnectionResponse
import worker.models.patient.PatientNhsNumber


class PatientRegistrationSteps : AbstractSteps() {

    @Given("^I have valid patient data to register new account$")
    fun iHaveValidPatientDataToRegisterNewAccount() {
        val patient = Patient(
                title = "Mr",
                firstName = "John",
                surname = "Smith",
                dateOfBirth = "1919-12-24T14:03:15.892Z",
                accountId = "1195029928",
                odsCode = odsCode,
                connectionToken = connectionToken,
                sessionId = "MT4vWCxTKXRYr7fFJWM3wB",
                endUserSessionId = "Ab42ZoP21dT4JE12avEWQ5",
                linkageKey = "KjwzyFSEUAGj4",
                userPatientLinkToken = "3v4DARxCmznF6eiGMQRR2u",
                nhsNumbers = listOf("7174450393")
        )

        mockingClient.forEmis { endUserSessionRequest().respondWithSuccess(patient.endUserSessionId) }
        mockingClient.forEmis { meRequest(patient).respondWithSuccess(patient.connectionToken) }
        mockingClient.forEmis { sessionRequest(patient).respondWithSuccess(patient, AssociationType.Self) }
        mockingClient.forEmis { demographicsRequest(patient).respondWithSuccess(patient, arrayOf(PatientIdentifier(patient.nhsNumbers[0], IdentifierType.NhsNumber))) }

        val connectionRequest = Im1ConnectionRequest()
        connectionRequest.AccountId = patient.accountId
        connectionRequest.LinkageKey = patient.linkageKey
        connectionRequest.OdsCode = patient.odsCode
        connectionRequest.Surname = patient.surname
        connectionRequest.DateOfBirth = patient.dateOfBirth

        setSessionVariable(Im1ConnectionRequest::class).to(connectionRequest)
        setSessionVariable("NHSNumbers").to(patient.nhsNumbers)
        setSessionVariable("ConnectionToken").to(patient.connectionToken)
        setSessionVariable("NationalPracticeCode").to(patient.odsCode)
    }

    @Given("^I have data for a patient that does not exist$")
    fun iHaveDataForAPatientThatDoesNotExist() {
        val patient = Patient(
                surname = "Smith",
                dateOfBirth = "1919-12-24T14:03:15Z",
                accountId = "1195029928",
                odsCode = odsCode,
                connectionToken = connectionToken,
                endUserSessionId = "zVfHuYArbENW4aoAUeQPyS",
                linkageKey = "KjwzyFSEUAGj4",
                nhsNumbers = listOf("notExistingNhsNumber")
        )

        mockingClient.forEmis { endUserSessionRequest().respondWithSuccess(patient.endUserSessionId) }
        mockingClient.forEmis { meRequest(patient).respondWithNoOnlineUserFound() }

        val connectionRequest = Im1ConnectionRequest()
        connectionRequest.AccountId = patient.accountId
        connectionRequest.LinkageKey = patient.linkageKey
        connectionRequest.OdsCode = patient.odsCode
        connectionRequest.Surname = patient.surname
        connectionRequest.DateOfBirth = patient.dateOfBirth

        setSessionVariable(Im1ConnectionRequest::class).to(connectionRequest)
        setSessionVariable("NHSNumbers").to(patient.nhsNumbers)
        setSessionVariable("ConnectionToken").to(patient.connectionToken)
        setSessionVariable("NationalPracticeCode").to(patient.odsCode)
    }

    @Given("^EMIS Demographics endpoint is disable$")
    fun emisDemographicsEndpointIsDisable() {
        val patient = Patient(
                surname = "Smith",
                dateOfBirth = "1919-12-24T14:03:15Z",
                accountId = "1195029928",
                odsCode = odsCode,
                endUserSessionId = "zVfHuYArbENW4aoAUeQPyS",
                linkageKey = "KjwzyFSEUAGj4"
        )

        mockingClient.forEmis { endUserSessionRequest().respondWithSuccess(patient.endUserSessionId) }
        mockingClient.forEmis { meRequest(patient).respondWithInvalidLinkLevel() }

        val connectionRequest = Im1ConnectionRequest()
        connectionRequest.AccountId = patient.accountId
        connectionRequest.LinkageKey = patient.linkageKey
        connectionRequest.OdsCode = patient.odsCode
        connectionRequest.Surname = patient.surname
        connectionRequest.DateOfBirth = patient.dateOfBirth
        setSessionVariable(Im1ConnectionRequest::class).to(connectionRequest)
    }

    @When("^I register an EMIS user's IM1 credentials$")
    fun iRegisterAnEMISUsersIMCredentials() {
        try {
            val result = sessionVariableCalled<WorkerClient>(WorkerClient::class)
                    .postIm1Connection(sessionVariableCalled<Im1ConnectionRequest>(Im1ConnectionRequest::class))
            setSessionVariable(Im1ConnectionResponse::class).to(result)
        } catch (httpException: NhsoHttpException) {
            setSessionVariable("HttpException").to(httpException)
        }
    }

    @Then("^I receive the expected connection token and single NHS Number$")
    fun iReceiveTheExpectedConnectionTokenAndSingleNHSNumber() {
        val result = sessionVariableCalled<Im1ConnectionResponse>(Im1ConnectionResponse::class)
        val nhsNumbers = sessionVariableCalled<List<String>>("NHSNumbers")
        val connectionToken = sessionVariableCalled<String>("ConnectionToken")

        Assert.assertNotNull("Exception thrown during IM1 connection post. Exception: ${sessionVariableCalled<NhsoHttpException>("HttpException")}", result)
        Assert.assertEquals(1, result.nhsNumbers!!.count())
        Assert.assertEquals(connectionToken, result.connectionToken)
        Assert.assertEquals(nhsNumbers[0], result.nhsNumbers!![0].nhsNumber)
    }

    @Given("^I have valid patient data with multiple nhs numbers to register new account$")
    fun iHaveValidPatientDataWithMultipleNhsNumbersToRegisterNewAccount() {
        val patient = Patient(
                title = "Miss",
                firstName = "Gertrude",
                surname = "Jones",
                dateOfBirth = "1965-08-12T00:00:00Z",
                accountId = "1195029928",
                odsCode = odsCode,
                connectionToken = connectionToken,
                sessionId = "DPcqihby6RDaVrnf4hHyv7",
                endUserSessionId = "zVGrzHH7YUPeEBRk1nat1D",
                linkageKey = "KjwzyFSEUAGj4",
                userPatientLinkToken = "BvN2TuCzPtHLZvv5ZgK6wN",
                nhsNumbers = listOf("nhsNumber1", "nhsNumber2")

        )
        mockingClient.forEmis { endUserSessionRequest().respondWithSuccess(patient.endUserSessionId) }
        mockingClient.forEmis { meRequest(patient).respondWithSuccess(connectionToken) }
        mockingClient.forEmis { sessionRequest(patient).respondWithSuccess(patient, AssociationType.Self) }
        mockingClient.forEmis {
            demographicsRequest(patient).respondWithSuccess(patient,
                    arrayOf(PatientIdentifier(patient.nhsNumbers[0]), PatientIdentifier(patient.nhsNumbers[1])))
        }

        val connectionRequest = Im1ConnectionRequest()
        connectionRequest.AccountId = patient.accountId
        connectionRequest.LinkageKey = patient.linkageKey
        connectionRequest.OdsCode = patient.odsCode
        connectionRequest.Surname = patient.surname
        connectionRequest.DateOfBirth = patient.dateOfBirth

        setSessionVariable(Im1ConnectionRequest::class).to(connectionRequest)
        setSessionVariable("NHSNumbers").to(patient.nhsNumbers)
        setSessionVariable("ConnectionToken").to(patient.connectionToken)
        setSessionVariable("NationalPracticeCode").to(patient.odsCode)
    }

    @Then("^I receive the expected connection token and multiple NHS Numbers$")
    fun iReceiveTheExpectedConnectionTokenAndMultipleNHSNumbers() {
        val result = sessionVariableCalled<Im1ConnectionResponse>(Im1ConnectionResponse::class)
        val nhsNumbers = sessionVariableCalled<List<String>>("NHSNumbers")
        val connectionToken = sessionVariableCalled<String>("ConnectionToken")

        Assert.assertNotNull("Exception thrown during IM1 connection post. Exception: ${sessionVariableCalled<NhsoHttpException>("HttpException")}", result)
        Assert.assertEquals(nhsNumbers[0], result.nhsNumbers!![0].nhsNumber)
        Assert.assertEquals(nhsNumbers[1], result.nhsNumbers!![1].nhsNumber)
        Assert.assertEquals(result.connectionToken, connectionToken)
    }

    @Given("^I have valid data for a patient with no NHS Number$")
    fun iHaveValidDataForAPatientWithNoNHSNumber() {
        val patient = Patient(
                title = "Mrs",
                firstName = "Jackie",
                surname = "Thompson",
                dateOfBirth = "2001-01-02T11:00:52Z",
                accountId = "1195029928",
                odsCode = odsCode,
                connectionToken = connectionToken,
                sessionId = "h3pYG9By2tVTqcvPvpw3DL",
                endUserSessionId = "zVGrzHH7YUPeEBRk1nat1D",
                linkageKey = "KjwzyFSEUAGj4",
                userPatientLinkToken = "5d4p6ZExhi97mmerMrtD5p"
        )

        mockingClient.forEmis { endUserSessionRequest().respondWithSuccess(patient.endUserSessionId) }
        mockingClient.forEmis { meRequest(patient).respondWithSuccess(patient.connectionToken) }
        mockingClient.forEmis { sessionRequest(patient).respondWithSuccess(patient, AssociationType.Self) }
        mockingClient.forEmis { demographicsRequest(patient).respondWithSuccess(patient, emptyArray()) }

        val connectionRequest = Im1ConnectionRequest()
        connectionRequest.AccountId = patient.accountId
        connectionRequest.LinkageKey = patient.linkageKey
        connectionRequest.OdsCode = patient.odsCode
        connectionRequest.Surname = patient.surname
        connectionRequest.DateOfBirth = patient.dateOfBirth

        setSessionVariable(Im1ConnectionRequest::class).to(connectionRequest)
        setSessionVariable("ConnectionToken").to(patient.connectionToken)
        setSessionVariable("NationalPracticeCode").to(patient.odsCode)
    }

    @Then("^I receive the expected connection token without NHS Numbers$")
    fun iReceiveTheExpectedConnectionTokenWithoutNHSNumbers() {
        val result = sessionVariableCalled<Im1ConnectionResponse>(Im1ConnectionResponse::class)
        val connectionToken = sessionVariableCalled<String>("ConnectionToken")

        Assert.assertNotNull("Exception thrown during IM1 connection post. Exception: ${sessionVariableCalled<NhsoHttpException>("HttpException")}", result)
        Assert.assertEquals(result.connectionToken, connectionToken)
        Assert.assertArrayEquals(result.nhsNumbers, emptyArray<PatientNhsNumber>())
    }

    @Given("^I have data for a patient that has already been associated with the application in the GP system$")
    fun iHaveDataForAPatientThatHasAlreadyBeenAssociatedWithTheApplicationInTheGPSystem() {
        val patient = Patient(
                surname = "AlreadyLinked",
                dateOfBirth = "1919-12-24T14:03:15Z",
                accountId = "1195029928",
                odsCode = odsCode,
                linkageKey = "KjwzyFSEUAGj4",
                endUserSessionId = "zVfHuYArbENW4aoAUeQPyS"
        )


        mockingClient.forEmis { endUserSessionRequest().respondWithSuccess(patient.endUserSessionId) }
        mockingClient.forEmis { meRequest(patient).respondWithUserAlreadyLinked() }

        val connectionRequest = Im1ConnectionRequest()
        connectionRequest.AccountId = patient.accountId
        connectionRequest.LinkageKey = patient.linkageKey
        connectionRequest.OdsCode = patient.odsCode
        connectionRequest.Surname = patient.surname
        connectionRequest.DateOfBirth = patient.dateOfBirth
        setSessionVariable(Im1ConnectionRequest::class).to(connectionRequest)
        setSessionVariable("HttpExceptionExpected").to(true)
    }
}