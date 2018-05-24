package features.sharedStepDefinitions.backend

import cucumber.api.java.en.Given
import mocking.emis.session.EmisEndUserSessionBuilder
import mocking.emis.me.EmisMeBuilder
import models.Patient

import net.serenitybdd.core.Serenity.setSessionVariable
import worker.models.patient.Im1ConnectionRequest

class PatientRegistrationValidationSteps : AbstractSteps() {
    val patient = Patient(
            surname = "Yoda",
            dateOfBirth = "1919-12-24T14:03:15.892",
            accountId = "MASTER_YODA",
            odsCode = OdsCode,
            endUserSessionId = "Ab42ZoP21dT4JE12avEWQ5",
            linkageKey = "MASTER000YODA"
    )

    private val INVALID_VALUE = "xxx-wrong-format-xxx"

    @Given("^I have an EMIS user's IM1 credentials with an ODS Code not in the expected format$")
    fun iHaveAnEMISUsersIMCredentialsWithAnODSCodeNotInTheExpectedFormat() {
        val connectionRequest = Im1ConnectionRequest()
        connectionRequest.AccountId = patient.accountId
        connectionRequest.LinkageKey = patient.linkageKey
        connectionRequest.OdsCode = INVALID_VALUE
        connectionRequest.Surname = patient.surname
        connectionRequest.DateOfBirth = patient.dateOfBirth

        setSessionVariable(Im1ConnectionRequest::class).to(connectionRequest)
    }

    @Given("^I have an EMIS user's IM1 credentials with a Surname not in the expected format$")
    fun iHaveAnEMISUsersIMCredentialsWithASurnameNotInTheExpectedFormat() {
        val patientWithInValidSurname = patient.copy(surname = INVALID_VALUE)
        mockingClient.forEmis { endUserSessionRequest().respondWithSuccess(patient.endUserSessionId) }
        mockingClient.forEmis {
            meRequest(patientWithInValidSurname)
                    .respondWithBadRequest("The Surname value cannot exceed 100 characters.")
        }

        val connectionRequest = Im1ConnectionRequest()
        connectionRequest.AccountId = patientWithInValidSurname.accountId
        connectionRequest.LinkageKey = patientWithInValidSurname.linkageKey
        connectionRequest.OdsCode = patientWithInValidSurname.odsCode
        connectionRequest.Surname = patientWithInValidSurname.surname
        connectionRequest.DateOfBirth = patientWithInValidSurname.dateOfBirth

        setSessionVariable(Im1ConnectionRequest::class).to(connectionRequest)
    }

    @Given("^I have an EMIS user's IM1 credentials with an Account ID not in the expected format$")
    fun iHaveAnEMISUsersIMCredentialsWithAnAccountIdNotInTheExpectedFormat() {
        val patientWithInvalidAccountId = patient.copy(accountId = INVALID_VALUE)
        mockingClient.forEmis { endUserSessionRequest().respondWithSuccess(patientWithInvalidAccountId.endUserSessionId) }
        mockingClient.forEmis {
            meRequest(patientWithInvalidAccountId)
                    .respondWithBadRequest("AccountId length outside of valid range. Must be between 10 - 15 (inclusive) characters.")
        }

        val connectionRequest = Im1ConnectionRequest()
        connectionRequest.AccountId = INVALID_VALUE
        connectionRequest.LinkageKey = patientWithInvalidAccountId.linkageKey
        connectionRequest.OdsCode = patientWithInvalidAccountId.odsCode
        connectionRequest.Surname = patientWithInvalidAccountId.surname
        connectionRequest.DateOfBirth = patientWithInvalidAccountId.dateOfBirth

        setSessionVariable(Im1ConnectionRequest::class).to(connectionRequest)
    }

    @Given("^I have an EMIS user's IM1 credentials with a Linkage Key not in the expected format$")
    fun iHaveAnEMISUsersIMCredentialsWithALinkageKeyNotInTheExpectedFormat() {
        val patientWithInvalidLinkageKey = patient.copy(linkageKey = INVALID_VALUE)
        mockingClient.forEmis { endUserSessionRequest().respondWithSuccess(patientWithInvalidLinkageKey.endUserSessionId) }
        mockingClient.forEmis {
            meRequest(patientWithInvalidLinkageKey)
                    .respondWithBadRequest("LinkageKey length outside of valid range. Must be between 6 - 15 (inclusive) characters.")
        }

        val connectionRequest = Im1ConnectionRequest()
        connectionRequest.AccountId = patientWithInvalidLinkageKey.accountId
        connectionRequest.LinkageKey = patientWithInvalidLinkageKey.linkageKey
        connectionRequest.OdsCode = patientWithInvalidLinkageKey.odsCode
        connectionRequest.Surname = patientWithInvalidLinkageKey.surname
        connectionRequest.DateOfBirth = patientWithInvalidLinkageKey.surname

        setSessionVariable(Im1ConnectionRequest::class).to(connectionRequest)
    }

    @Given("^I have an EMIS user's IM1 credentials with a Date Of Birth not in the expected format$")
    fun iHaveAnEMISUsersIMCredentialsWithADateOfBirthNotInTheExpectedFormat() {
        val connectionRequest = Im1ConnectionRequest()
        connectionRequest.AccountId = patient.accountId
        connectionRequest.LinkageKey = patient.linkageKey
        connectionRequest.OdsCode = patient.odsCode
        connectionRequest.Surname = patient.surname
        connectionRequest.DateOfBirth = INVALID_VALUE

        setSessionVariable(Im1ConnectionRequest::class).to(connectionRequest)
    }

    @Given("^I have an EMIS user's IM1 credentials with missing ODS Code$")
    fun iHaveAnEMISUsersIMCredentialsWithMissingODSCode() {
        val connectionRequest = Im1ConnectionRequest()
        connectionRequest.AccountId = patient.accountId
        connectionRequest.LinkageKey = patient.linkageKey
        connectionRequest.Surname = patient.surname
        connectionRequest.DateOfBirth = patient.dateOfBirth

        setSessionVariable(Im1ConnectionRequest::class).to(connectionRequest)
    }

    @Given("^I have an EMIS user's IM1 credentials with missing Surname$")
    fun iHaveAnEMISUsersIMCredentialsWithMissingSurname() {
        val connectionRequest = Im1ConnectionRequest()
        connectionRequest.AccountId = patient.accountId
        connectionRequest.LinkageKey = patient.linkageKey
        connectionRequest.OdsCode = patient.odsCode
        connectionRequest.DateOfBirth = patient.dateOfBirth

        setSessionVariable(Im1ConnectionRequest::class).to(connectionRequest)
    }

    @Given("^I have an EMIS user's IM1 credentials with missing Account ID$")
    fun iHaveAnEMISUsersIMCredentialsWithMissingAccountID() {
        val connectionRequest = Im1ConnectionRequest()
        connectionRequest.LinkageKey = patient.linkageKey
        connectionRequest.OdsCode = patient.odsCode
        connectionRequest.Surname = patient.surname
        connectionRequest.DateOfBirth = patient.dateOfBirth

        setSessionVariable(Im1ConnectionRequest::class).to(connectionRequest)
    }

    @Given("^I have an EMIS user's IM1 credentials with missing Linkage Key$")
    fun iHaveAn_EMISUsersIMCredentialsWithMissingLinkageKey() {
        val connectionRequest = Im1ConnectionRequest()
        connectionRequest.AccountId = patient.accountId
        connectionRequest.OdsCode = patient.odsCode
        connectionRequest.Surname = patient.surname
        connectionRequest.DateOfBirth = patient.dateOfBirth

        setSessionVariable(Im1ConnectionRequest::class).to(connectionRequest)
    }
}
