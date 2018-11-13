package features.sharedStepDefinitions.backend

import cucumber.api.java.en.Given
import cucumber.api.java.en.Then
import mocking.defaults.EmisMockDefaults
import models.Patient
import net.serenitybdd.core.Serenity.sessionVariableCalled
import net.serenitybdd.core.Serenity.setSessionVariable
import org.junit.Assert
import worker.NhsoHttpException
import worker.models.patient.Im1ConnectionRequest
import worker.models.patient.Im1ConnectionResponse
import worker.models.patient.PatientNhsNumber


class PatientRegistrationSteps : AbstractSteps() {


    @Given("^EMIS Demographics endpoint is disable$")
    fun emisDemographicsEndpointIsDisable() {
        val patient = Patient(
                surname = "Smith",
                dateOfBirth = "1919-12-24T14:03:15Z",
                accountId = "1195029928",
                odsCode = EmisMockDefaults.DEFAULT_ODS_CODE_EMIS,
                endUserSessionId = "zVfHuYArbENW4aoAUeQPyS",
                linkageKey = "KjwzyFSEUAGj4"
        )

        mockingClient.forEmis { authentication.endUserSessionRequest().respondWithSuccess(patient.endUserSessionId) }
        mockingClient.forEmis { authentication.meRequest(patient).respondWithInvalidLinkLevel() }

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

        Assert.assertNotNull("Exception thrown during IM1 connection post. " +
                "Exception: ${sessionVariableCalled<NhsoHttpException>("HttpException")}", result)
        Assert.assertEquals(nhsNumbers[0], result.nhsNumbers!![0].nhsNumber)
        Assert.assertEquals(nhsNumbers[1], result.nhsNumbers!![1].nhsNumber)
        Assert.assertEquals(result.connectionToken, connectionToken)
    }

    @Then("^I receive the expected connection token without NHS Numbers$")
    fun iReceiveTheExpectedConnectionTokenWithoutNHSNumbers() {
        val result = sessionVariableCalled<Im1ConnectionResponse>(Im1ConnectionResponse::class)
        val connectionToken = sessionVariableCalled<String>("ConnectionToken")

        Assert.assertNotNull("Exception thrown during IM1 connection post. " +
                "Exception: ${sessionVariableCalled<NhsoHttpException>("HttpException")}", result)
        Assert.assertEquals(result.connectionToken, connectionToken)
        Assert.assertArrayEquals(result.nhsNumbers, emptyArray<PatientNhsNumber>())
    }
}
