package features.sharedStepDefinitions.backend

import cucumber.api.java.en.Given
import cucumber.api.java.en.Then
import cucumber.api.java.en.When
import mocking.emis.demographics.PatientIdentifier

import mocking.emis.models.AssociationType
import mocking.emis.models.IdentifierType
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

        val connectionRequest = Im1ConnectionRequest(
                AccountId = patient.accountId,
                LinkageKey = patient.linkageKey,
                OdsCode = patient.odsCode,
                Surname = patient.surname,
                DateOfBirth = patient.dateOfBirth)

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

        val connectionRequest = Im1ConnectionRequest(
                AccountId = patient.accountId,
                LinkageKey = patient.linkageKey,
                OdsCode = patient.odsCode,
                Surname = patient.surname,
                DateOfBirth = patient.dateOfBirth)
        setSessionVariable(Im1ConnectionRequest::class).to(connectionRequest)
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

        val connectionRequest = Im1ConnectionRequest(
                AccountId = patient.accountId,
                LinkageKey = patient.linkageKey,
                OdsCode = patient.odsCode,
                Surname = patient.surname,
                DateOfBirth = patient.dateOfBirth)
        setSessionVariable(Im1ConnectionRequest::class).to(connectionRequest)
        setSessionVariable("HttpExceptionExpected").to(true)
    }
}