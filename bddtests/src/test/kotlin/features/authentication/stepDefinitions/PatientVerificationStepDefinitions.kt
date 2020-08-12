package features.authentication.stepDefinitions

import constants.Supplier
import io.cucumber.java.en.Given
import io.cucumber.java.en.Then
import io.cucumber.java.en.When
import features.authentication.factories.PatientVerificationFactory
import mocking.MockingClient
import mocking.defaults.EmisMockDefaults
import mocking.defaults.VisionMockDefaults
import mocking.emis.demographics.PatientIdentifier
import mocking.vision.models.VisionUserSession
import models.NhsNumberFormatter
import models.Patient
import net.serenitybdd.core.Serenity.sessionVariableCalled
import net.serenitybdd.core.Serenity.setSessionVariable
import org.junit.Assert
import utils.SerenityHelpers
import utils.getOrFail
import utils.set
import worker.WorkerClient
import worker.models.patient.Im1ConnectionResponse

class PatientVerificationStepDefinitions {

    private val mockingClient = MockingClient.instance

    @Given("^I have an (.*) IM1 Connection Token that does not exist$")
    fun givenIHaveAnImConnectionTokenThatDoesNotExist(gpSystem: String) {
        val supplier = Supplier.valueOf(gpSystem)
        SerenityHelpers.setPatient(Patient.getDefault(supplier))
        PatientVerificationFactory.getForSupplier(supplier).im1ConnectionTokenDoesNotExist()
    }

    @Given("^Vision responds with a connection to external service failed$")
    fun givenVisionRespondsWithAConnectionToExternalServiceFailed() {
        PatientVerificationFactory.getForSupplier(Supplier.VISION).connectionToExternalServiceFailed()
    }

    @Given("^I have an (.*) IM1 Connection Token that is in an invalid format$")
    fun givenIHaveAnIm1ConnectionTokenThatIsInAnInvalidFormat(gpSystem: String) {
        val supplier = Supplier.valueOf(gpSystem)
        PatientVerificationSerenityHelpers.ConnectionToken.set("token")
        setDefaultNationalPracticeCodeSessionVariable(supplier)
    }

    @Given("^I have an EMIS IM1 Connection Token and I try to verify as a microtest user$")
    fun givenIHaveAnIm1ConnectionTokenForEmisAndIAmMicrotest() {
        PatientVerificationSerenityHelpers.ConnectionToken.set(EmisMockDefaults.DEFAULT_CONNECTION_TOKEN)
        setDefaultNationalPracticeCodeSessionVariable(Supplier.MICROTEST)
    }

    private fun setDefaultNationalPracticeCodeSessionVariable(supplier: Supplier) {
        val odsCode = PatientVerificationFactory.getForSupplier(supplier).odsCode
        PatientVerificationSerenityHelpers.NationalPracticeCode.set(odsCode)
    }

    @Given("^Vision responds with a security header error$")
    fun visionRespondsWithASecurityHeaderError() {
        PatientVerificationSerenityHelpers.ConnectionToken.set(VisionMockDefaults.patientVision.connectionToken)
        PatientVerificationSerenityHelpers.NationalPracticeCode.set(VisionMockDefaults.DEFAULT_ODS_CODE_VISION)
        mockingClient.forVision.mock {
                    authentication.getConfigurationRequest(
                            visionUserSession = VisionUserSession.fromPatient(Patient.getDefault(Supplier.VISION)))
                            .respondWithSecurityHeaderError()
                }
    }

    @Given("^Vision responds with an invalid request error$")
    fun visionRespondsWithAInvalidRequestError() {
        PatientVerificationSerenityHelpers.ConnectionToken.set(VisionMockDefaults.patientVision.connectionToken)
        PatientVerificationSerenityHelpers.NationalPracticeCode.set(VisionMockDefaults.DEFAULT_ODS_CODE_VISION)

        mockingClient.forVision.mock {
                    authentication.getConfigurationRequest(
                            visionUserSession = VisionUserSession.fromPatient(Patient.getDefault(Supplier.VISION)))
                            .respondWithInvalidRequest()
                }
    }

    @Given("^Vision responds with an unknown error$")
    fun visionRespondsWithAnUnknownError() {
        val patient =  Patient.getDefault(Supplier.VISION)
        PatientVerificationSerenityHelpers.ConnectionToken.set(patient.connectionToken)
        PatientVerificationSerenityHelpers.NationalPracticeCode.set(patient.odsCode)
        mockingClient.forVision.mock {
                    authentication.getConfigurationRequest(
                            VisionMockDefaults.getVisionUserSession(patient))
                            .respondWithUnknownError()
                }
    }

    @Given("^Vision responds with a record currently unavailable error$")
    fun visionRespondsWithARecordCurrentlyUnavailableError() {
        val patient =  Patient.getDefault(Supplier.VISION)
        PatientVerificationSerenityHelpers.ConnectionToken.set(patient.connectionToken)
        PatientVerificationSerenityHelpers.NationalPracticeCode.set(patient.odsCode)
        mockingClient.forVision.mock {
                    authentication.getConfigurationRequest(
                            VisionMockDefaults.getVisionUserSession(patient))
                            .respondWithRecordCurrentlyUnavailableError()
                }
    }

    @Given("^Vision responds with a registration incomplete error$")
    fun visionRespondsWithARegistrationIncompleteError() {
        val patient =  Patient.getDefault(Supplier.VISION)
        PatientVerificationSerenityHelpers.ConnectionToken.set(patient.connectionToken)
        PatientVerificationSerenityHelpers.NationalPracticeCode.set(patient.odsCode)
        mockingClient.forVision.mock {
            authentication.getConfigurationRequest(
                    VisionMockDefaults.getVisionUserSession(patient))
                    .respondWithRegistrationIncomplete()
        }
    }

    @Given("^I have an (.*) ODS Code not in expected format$")
    fun givenIHaveAnOdsCodeNotInExpectedFormat(gpSystem: String) {
        val supplier = Supplier.valueOf(gpSystem)
        PatientVerificationSerenityHelpers.ConnectionToken.set(Patient.getDefault(supplier).connectionToken)
        PatientVerificationSerenityHelpers.NationalPracticeCode.set("£$*&")
    }

    @Given("^I have an (.*) ODS Code that does not exist$")
    fun givenIHaveAnOdsCodeThatDoesNotExists(gpSystem: String) {

        val supplier = Supplier.valueOf(gpSystem)
        PatientVerificationSerenityHelpers.ConnectionToken.set(Patient.getDefault(supplier).connectionToken)
        PatientVerificationSerenityHelpers.NationalPracticeCode.set("E99999")
    }


    @Given("^I have valid credentials for a (.*) patient with one NHS Number$")
    fun givenIHaveValidCredentialsForAPatientWithOneNhsNumber(gpSystem: String) {
        val supplier = Supplier.valueOf(gpSystem)
        PatientVerificationFactory.getForSupplier(supplier).validPatientWithOneNhsNumber()
    }

    @Given("^I have valid credentials for a (.*) patient with multiple NHS Numbers$")
    fun givenIHaveValidCredentialsForAPatientWithMultipleNhsNumbers(gpSystem: String) {
        val supplier = Supplier.valueOf(gpSystem)
        PatientVerificationFactory.getForSupplier(supplier).validPatientWithMultipleNumbers()
    }

    @Given("^I have valid credentials for a (.*) patient with no NHS Number$")
    fun givenIHaveValidCredentialsForAPatientWithNoNhsNumber(gpSystem: String) {
        val supplier = Supplier.valueOf(gpSystem)
        PatientVerificationFactory.getForSupplier(supplier).validPatientWithNoNhsNumber()
    }

    @Given("^I have an old ODS Code and IM1 Connection Token for a (\\w*) patient that has since moved to a"
            + " different practice$")
    fun givenIHaveAnOldOdsCodeAndIm1ConnectionTokenForAPatientThatHasSinceMovedToADifferentPractice(gpSystem: String) {
        val supplier = Supplier.valueOf(gpSystem)
        PatientVerificationFactory.getForSupplier(supplier)
                                  .oldOdsCodeAndConnectionTokenForPatientThatHasSinceMovedToADifferentPractice()
    }

    @Given("I have valid a valid IM1 Connection Token for a (\\w+) patient but the GP System is not available$")
    fun giveIHaveAValidIM1ConnectionTokenForAPatientButTheGpSystemIsNotAvailable(gpSystem: String) {
        val supplier = Supplier.valueOf(gpSystem)
        val patient = Patient.getDefault(supplier)
        SerenityHelpers.setPatient(patient)
        PatientVerificationFactory.getForSupplier(supplier).gpSystemNotAvailable()
    }

    @When("^I verify patient data using the v1 endpoint$")
    fun whenIVerifyPatientData() {
        val connectionToken = PatientVerificationSerenityHelpers.ConnectionToken.getOrFail<String>()
        val odsCode = PatientVerificationSerenityHelpers.NationalPracticeCode.getOrFail<String>()

        whenIVerifyPatientData(connectionToken, odsCode)
    }

    @When("^I verify patient data without sending the IM1 Connection Token$")
    fun whenIVerifyPatientDataWithoutSendingTheIm1ConnectionToken() {
        val odsCode = PatientVerificationSerenityHelpers.NationalPracticeCode.getOrFail<String>()

        whenIVerifyPatientData(null, odsCode)
    }

    @When("^I verify patient data without sending the ODS Code$")
    fun whenIVerifyPatientDataWithoutSendingTheOdsCode() {
        val connectionToken = PatientVerificationSerenityHelpers.ConnectionToken.getOrFail<String>()

        whenIVerifyPatientData(connectionToken, null)
    }

    private fun whenIVerifyPatientData(connectionToken: String?, odsCode: String?) {
        val result = sessionVariableCalled<WorkerClient>(WorkerClient::class)
                .authentication.getIm1Connection(connectionToken, odsCode)
        setSessionVariable(Im1ConnectionResponse::class).to(result)
    }

    @Then("^I receive the expected NHS Numbers?$")
    fun thenIReceiveTheExpectedNhsNumber() {
        val result = sessionVariableCalled<Im1ConnectionResponse>(Im1ConnectionResponse::class)
        val nhsNumbers = PatientVerificationSerenityHelpers.NhsNumbers.getOrFail<Array<PatientIdentifier>>()
        val connectionToken = PatientVerificationSerenityHelpers.ConnectionToken.getOrFail<String>()

        Assert.assertNotNull(result)
        val resultNhsNumbers = result.nhsNumbers!!.map {
            number -> NhsNumberFormatter.format(number.nhsNumber) }
                .toTypedArray()
        val expectedNhsNumbers = nhsNumbers.map {
            number -> NhsNumberFormatter.format(number.identifierValue!!) }
                .toTypedArray()
        Assert.assertEquals(nhsNumbers.count(), resultNhsNumbers.count())
        Assert.assertArrayEquals("Expected NHS Numbers", expectedNhsNumbers, resultNhsNumbers)
        Assert.assertEquals(connectionToken, result.connectionToken)
    }

    @Then("^I receive no NHS Number$")
    fun thenIReceiveNoNhsNumber() {
        val result = sessionVariableCalled<Im1ConnectionResponse>(Im1ConnectionResponse::class)
        Assert.assertNotNull("IM1 connection response expected, but was null", result)
        Assert.assertEquals(0, result.nhsNumbers!!.count())
    }
}
