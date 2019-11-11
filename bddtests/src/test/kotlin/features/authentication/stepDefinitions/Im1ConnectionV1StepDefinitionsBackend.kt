package features.authentication.stepDefinitions

import cucumber.api.java.en.Given
import cucumber.api.java.en.Then
import cucumber.api.java.en.When
import features.myrecord.factories.DemographicsFactory
import mocking.GsonFactory
import mocking.MockingClient
import mocking.defaults.dataPopulation.journies.im1Connection.SuccessfulRegistrationJourney
import mocking.defaults.dataPopulation.journies.session.CitizenIdSessionCreateJourney
import mocking.defaults.dataPopulation.journies.session.SessionCreateJourneyFactory
import mocking.emis.me.LinkApplicationRequestModel
import mocking.emis.me.LinkageDetailsModel
import mockingFacade.linkage.LinkageInformationFacade
import models.Patient
import models.patients.EmisPatients
import mongodb.MongoDBConnection
import net.serenitybdd.core.Serenity
import net.serenitybdd.core.Serenity.setSessionVariable
import org.junit.Assert
import utils.GlobalSerenityHelpers
import utils.SerenityHelpers
import utils.getOrFail
import utils.getOrNull
import utils.set
import worker.NhsoHttpException
import worker.WorkerClient
import worker.models.linkage.LinkageResponse
import worker.models.patient.Im1ConnectionRequest
import worker.models.patient.Im1ConnectionResponse
import worker.models.patient.Im1ConnectionToken

class Im1ConnectionV1StepDefinitionsBackend {
    val mockingClient = MockingClient.instance

    @Given("^I have a new (.+) patient with Nhs Numbers of (.*)$")
    fun iHaveValidPatientDataToRegisterNewAccount(gpSystem: String, nhsNumbers: String) {
        val nhsNumbersList = nhsNumbers.split(",").filter { it.isNotEmpty() }
        val patient = Patient.getDefault(gpSystem).copy(nhsNumbers = nhsNumbersList)
        SerenityHelpers.setPatient(patient)
        DemographicsFactory.getForSupplier(gpSystem).enabled(patient)
        SuccessfulRegistrationJourney(mockingClient).create(patient, gpSystem)
        setIm1Request(patient)
    }

    @Given("^I have data for a (.+) patient that does not exist$")
    fun iHaveDataForAPatientThatDoesNotExist(gpSystem: String) {
        val patient= Patient.getDefault(gpSystem).copy(nhsNumbers = arrayListOf("nonExistingNhsNumber"))
        AuthenticationFactory.getForSupplier(gpSystem).patientDoesNotExist(patient)
        setIm1Request(patient)
    }

    @Given("^I have data for a (.+) patient with incorrect linkage key$")
    fun iHaveDataForAPatientWithIncorrectLinkageKey(gpSystem: String) {
        val patient= Patient.getDefault(gpSystem).copy(linkageKey = "incorrectLinkageKey")
        AuthenticationFactory.getForSupplier(gpSystem).patientWithIncorrectLinkageKey(patient)
        setIm1Request(patient)
    }

    @Given("^I have data for a (.+) patient with incorrect surname$")
    fun iHaveDataForAPatientWithIncorrectSurname(gpSystem: String) {
        val patient = Patient.getDefault(gpSystem).copy(surname = "incorrectSurname")
        AuthenticationFactory.getForSupplier(gpSystem).patientWithIncorrectSurname(patient)
        setIm1Request(patient)
    }

    @Given("^I have data for a (.+) patient with incorrect date of birth$")
    fun iHaveDataForAPatientWithIncorrectDateOfBirth(gpSystem: String) {
        val patient = Patient.getDefault(gpSystem).copy(surname = "1900-01-01")
        AuthenticationFactory.getForSupplier(gpSystem).patientWithIncorrectDOB(patient)
        setIm1Request(patient)
    }

    @Given("^I have a user's IM1 credentials with an ODS Code not in the expected format$")
    fun iHaveAUsersIMCredentialsWithAnODSCodeNotInTheExpectedFormat() {
        val patient = Patient.getDefault("EMIS").copy(odsCode = INVALID_VALUE)
        setIm1Request(patient)
    }

    @Given("^I have a (.+) user's IM1 credentials with a Surname not in the expected format$")
    fun iHaveAnEMISUsersIMCredentialsWithASurnameNotInTheExpectedFormat(gpSystem: String) {
        val patient = Patient.getDefault(gpSystem).copy(surname = INVALID_VALUE)
        AuthenticationFactory.getForSupplier(gpSystem).patientWithSurnameInWrongFormat(patient)
        setIm1Request(patient)
    }

    @Given("^I have a (.+) user's IM1 credentials with an Account ID not in the expected format$")
    fun iHaveAnEMISUsersIMCredentialsWithAnAccountIdNotInTheExpectedFormat(gpSystem: String) {
        val patient:Patient
        if (gpSystem == "VISION") {
            patient = Patient.getDefault(gpSystem).copy(rosuAccountId = "10496")
        } else {
            patient = Patient.getDefault(gpSystem).copy(accountId = INVALID_VALUE)
        }
        AuthenticationFactory.getForSupplier(gpSystem).patientWithAccountIDInWrongFormat(patient)
        setIm1Request(patient)
    }

    @Given("^I have a (.+) user's IM1 credentials with a Linkage Key not in the expected format$")
    fun iHaveAnEMISUsersIMCredentialsWithALinkageKeyNotInTheExpectedFormat(gpSystem: String) {
        val patient = Patient.getDefault(gpSystem).copy(linkageKey = INVALID_VALUE)
        AuthenticationFactory.getForSupplier(gpSystem).patientWithLinkageKeyInWrongFormat(patient)
        setIm1Request(patient)
    }

    @Given("^I have a (.+) user's IM1 credentials with a Date Of Birth not in the expected format$")
    fun iHaveAnEMISUsersIMCredentialsWithADateOfBirthNotInTheExpectedFormat(gpSystem: String) {
        val patient = Patient.getDefault(gpSystem).copy(dateOfBirth = INVALID_VALUE)
        AuthenticationFactory.getForSupplier(gpSystem).patientWithDOBInWrongFormat(patient)
        setIm1Request(patient)
    }

    @Given("^I have a user's IM1 credentials with missing ODS Code$")
    fun iHaveAnEMISUsersIMCredentialsWithMissingODSCode() {
        val patient = EmisPatients.johnSmith
        SerenityHelpers.setPatient(patient)
        AuthenticationSerenityHelpers.IM1_CONNECTION_REQUEST.set(Im1ConnectionRequest(
                AccountId = patient.accountId,
                LinkageKey = patient.linkageKey,
                Surname = patient.surname,
                DateOfBirth = patient.dateOfBirth))
    }

    @Given("^I have data for an EMIS patient that has already been associated with the application in the GP system$")
    fun iHaveDataForAnEMISPatientThatHasAlreadyBeenAssociatedWithTheApplicationInTheGPSystem() {
        val patient = Patient.getDefault("EMIS")
        mockingClient.forEmis { authentication.endUserSessionRequest().respondWithSuccess(patient.endUserSessionId) }
        mockingClient.forEmis {
            authentication.meApplicationsRequest(patient, createLinkApplicationRequestModel(patient))
                    .respondWithAlreadyLinked()
        }
        setIm1Request(patient)
        setSessionVariable("HttpExceptionExpected").to(true)
    }

    private fun createLinkApplicationRequestModel(patient: Patient): LinkApplicationRequestModel {
        return LinkApplicationRequestModel(
                surname = patient.surname,
                dateOfBirth = patient.dateOfBirth.plus("T00:00:00"),
                linkageDetails = LinkageDetailsModel(
                        accountId = patient.accountId,
                        nationalPracticeCode = patient.odsCode,
                        linkageKey = patient.linkageKey
                )
        )
    }

    @Given("^I have data for a Vision patient and Vision returns with \"(.*)\"$")
    fun iHaveDataForAVisionPatientThatReturnsWith(errorText: String) {
        val patient = Patient.getDefault("VISION")
        AuthenticationFactoryVision.createInvalidTestForVision(patient, errorText)
        setIm1Request(patient)
        setSessionVariable("HttpExceptionExpected").to(true)
    }

    private fun setIm1Request(patient:Patient) {
        AuthenticationSerenityHelpers.IM1_CONNECTION_REQUEST.set(Im1ConnectionRequest(
                AccountId = patient.accountId,
                LinkageKey = patient.linkageKey,
                OdsCode = patient.odsCode,
                Surname = patient.surname,
                DateOfBirth = patient.dateOfBirth))
    }

    @Given("^I have a (.+) user's IM1 credentials with missing Surname$")
    fun iHaveAnEMISUsersIMCredentialsWithMissingSurname(gpSystem: String) {
        val patient = Patient.getDefault(gpSystem)
        SerenityHelpers.setPatient(patient)
        AuthenticationSerenityHelpers.IM1_CONNECTION_REQUEST.set(Im1ConnectionRequest(
                AccountId = patient.accountId,
                LinkageKey = patient.linkageKey,
                OdsCode = patient.odsCode,
                DateOfBirth = patient.dateOfBirth))
    }

    @Given("^I have a (.+) user's IM1 credentials with missing Account ID$")
    fun iHaveAnEMISUsersIMCredentialsWithMissingAccountID(gpSystem: String) {
        val patient = Patient.getDefault(gpSystem)
        SerenityHelpers.setPatient(patient)
        AuthenticationSerenityHelpers.IM1_CONNECTION_REQUEST.set(Im1ConnectionRequest(
                LinkageKey = patient.linkageKey,
                OdsCode = patient.odsCode,
                Surname = patient.surname,
                DateOfBirth = patient.dateOfBirth))
    }

    @Given("^I have a (.+) user's IM1 credentials with missing Linkage Key$")
    fun iHaveAnEMISUsersIMCredentialsWithMissingLinkageKey(gpSystem: String) {
        val patient = Patient.getDefault(gpSystem)
        SerenityHelpers.setPatient(patient)
        AuthenticationSerenityHelpers.IM1_CONNECTION_REQUEST.set(Im1ConnectionRequest(
                AccountId = patient.accountId,
                OdsCode = patient.odsCode,
                Surname = patient.surname,
                DateOfBirth = patient.dateOfBirth))
    }

    @Given("^I have a microtest user's IM1 credentials with a emis connection token$")
    fun iHaveAMicrotestUsersIMCredentialsEmisConnectionToken(gpSystem: String) {
        val patient = Patient.getDefault(gpSystem)
        SerenityHelpers.setPatient(patient)
        AuthenticationSerenityHelpers.IM1_CONNECTION_REQUEST.set(Im1ConnectionRequest(
                AccountId = patient.accountId,
                OdsCode = patient.odsCode,
                Surname = patient.surname,
                DateOfBirth = patient.dateOfBirth
        ))
    }

    @Given("^no IM1 Connection Token is currently cached$")
    fun im1ConnectionTokensClearedFromTheCache() {
        MongoDBConnection.Im1CacheCollection.clearCache()
    }

    @When("^I register the user's IM1 credentials$")
    fun iRegisterAUsersIMCredentials() {
        try {
            val im1ConnectionRequest = AuthenticationSerenityHelpers.IM1_CONNECTION_REQUEST
                    .getOrFail<Im1ConnectionRequest>()
            val result = Serenity.sessionVariableCalled<WorkerClient>(WorkerClient::class)
                    .authentication.postIm1Connection(im1ConnectionRequest)
            setSessionVariable(Im1ConnectionResponse::class).to(result)
            AuthenticationSerenityHelpers.IM1_CONNECTION_RESPONSE.set(result)
        } catch (httpException: NhsoHttpException) {
            SerenityHelpers.setHttpException(httpException)
        }
    }

    @When("^I register the Microtest user's IM1 credentials after linkage$")
    fun iRegisterTheMicrotestUsersIMCredentialsAfterLinkage() {
        val linkageResponse = Serenity.sessionVariableCalled<LinkageResponse>(LinkageResponse::class)
        val linkage = Serenity.sessionVariableCalled<LinkageInformationFacade>(LinkageInformationFacade::class)
        Assert.assertNotNull(linkageResponse)
        Assert.assertEquals(linkage.odsCode, linkageResponse.odsCode)
        val patient = SerenityHelpers.getPatient()
        DemographicsFactory.getForSupplier("MICROTEST").enabled(Patient.getDefault("MICROTEST"))
        SuccessfulRegistrationJourney(mockingClient).create(patient, "MICROTEST")
        setIm1Request(patient)
        try {
            val im1ConnectionRequest = AuthenticationSerenityHelpers.IM1_CONNECTION_REQUEST
                    .getOrFail<Im1ConnectionRequest>()
            val result = Serenity.sessionVariableCalled<WorkerClient>(WorkerClient::class)
                    .authentication.postIm1Connection(im1ConnectionRequest)
            setSessionVariable(Im1ConnectionResponse::class).to(result)
            AuthenticationSerenityHelpers.IM1_CONNECTION_RESPONSE.set(result)
        } catch (httpException: NhsoHttpException) {
            SerenityHelpers.setHttpException(httpException)
        }
    }

    @When("^I POST to IM1 Connection to register the user$")
    fun iPostToIm1Connection() {
        val linkingInformationExample =
                Serenity.sessionVariableCalled<LinkageInformationFacade>(LinkageInformationFacade::class)
        AuthenticationSerenityHelpers.IM1_CONNECTION_REQUEST.set(Im1ConnectionRequest(
                linkingInformationExample.accountId,
                linkingInformationExample.linkageKey,
                linkingInformationExample.odsCode,
                linkingInformationExample.surname,
                linkingInformationExample.dateOfBirth
        ))
        val gpSystem = GlobalSerenityHelpers.GP_SYSTEM.getOrFail<String>()
        val patient = Patient.getDefault(gpSystem).copy(
                accountId = linkingInformationExample.accountId,
                linkageKey = linkingInformationExample.linkageKey,
                odsCode = linkingInformationExample.odsCode,
                surname = linkingInformationExample.surname,
                dateOfBirth = linkingInformationExample.dateOfBirth,
                nhsNumbers = arrayListOf(linkingInformationExample.nhsNumber)
        )
        CitizenIdSessionCreateJourney(mockingClient).createFor(patient)
        SessionCreateJourneyFactory.getForSupplier(gpSystem, mockingClient).createFor(patient)
        SuccessfulRegistrationJourney(mockingClient).create(patient, gpSystem)
        iRegisterAUsersIMCredentials()
        val im1ConnectionResponse = AuthenticationSerenityHelpers.IM1_CONNECTION_RESPONSE
                .getOrNull<Im1ConnectionResponse>()
        val errorResponse = SerenityHelpers.getHttpResponse()
        Assert.assertNotNull("IM1 Connection Token post failed: $errorResponse", im1ConnectionResponse)
    }

    @When("^I have logged in with the user associated with the IM1 Connection Token$")
    fun loggedInWithTheUserAssociatedWithTheIm1ConnectionToken() {
        val patient = SerenityHelpers.getPatient()
        Assert.assertNotNull(Serenity.sessionVariableCalled<WorkerClient>(WorkerClient::class).authentication
                .postSessionConnection(patient.cidUserSession))
    }

    @Then("^the response has the expected connection token$")
    fun theResponseHasTheExpectedConnectionToken() {
        val result = AuthenticationSerenityHelpers.IM1_CONNECTION_RESPONSE
                .getOrNull<Im1ConnectionResponse>()
        val patient = SerenityHelpers.getPatient()
        val expectedIm1ConnectionToken = patient.im1ConnectionToken
        val actualIm1ConnectionToken = GsonFactory.asPascal.fromJson<Im1ConnectionToken>(
                result?.connectionToken,
                Im1ConnectionToken::class.java
        )
        Assert.assertEquals(expectedIm1ConnectionToken, actualIm1ConnectionToken)
    }

    @Then("^the response has the expected NHS numbers$")
    fun theResponseHasTheExpectedNhsNumbers() {
        val response = AuthenticationSerenityHelpers.IM1_CONNECTION_RESPONSE
                .getOrNull<Im1ConnectionResponse>()
        val responseNhsNumbers = response!!.nhsNumbers!!.map { it.nhsNumber.replace(" ", "") }
        val patient = SerenityHelpers.getPatient()
        Assert.assertEquals(patient.nhsNumbers, responseNhsNumbers)
    }

    @Then("^the IM1 Connection Token is in the cache$")
    fun theIm1ConnectionTokenIsInTheCache() {
        MongoDBConnection.Im1CacheCollection.assertNumberOfDocuments(1)
    }

    @Then("^the IM1 Connection Token is no longer in the cache$")
    fun theIm1ConnectionTokenIsNoLongerInTheCache() {
        MongoDBConnection.Im1CacheCollection.assertNumberOfDocuments(0)
    }
}
