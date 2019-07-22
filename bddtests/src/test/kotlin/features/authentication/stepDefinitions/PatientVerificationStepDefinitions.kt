package features.authentication.stepDefinitions

import cucumber.api.java.en.Given
import cucumber.api.java.en.Then
import cucumber.api.java.en.When
import features.authentication.factories.PatientVerificationFactory
import features.sharedStepDefinitions.backend.AbstractSteps
import mocking.emis.demographics.PatientIdentifier
import mocking.defaults.VisionMockDefaults
import mocking.vision.models.VisionUserSession
import models.NhsNumberFormatter
import models.Patient
import net.serenitybdd.core.Serenity.sessionVariableCalled
import net.serenitybdd.core.Serenity.setSessionVariable
import org.junit.Assert
import worker.NhsoHttpException
import worker.WorkerClient
import worker.models.patient.Im1ConnectionResponse

class PatientVerificationStepDefinitions : AbstractSteps() {

    @Given("I have an (.*) IM1 Connection Token that does not exist")
    fun givenIHaveAnImConnectionTokenThatDoesNotExist(gpSystem: String) {
        PatientVerificationFactory.getForSupplier(gpSystem).im1ConnectionTokenDoesNotExist()
    }

    @Given("I have an (.*) IM1 Connection Token that is in an invalid format")
    fun givenIHaveAnIm1ConnectionTokenThatIsInAnInvalidFormat(gpSystem: String) {
        setSessionVariable("ConnectionToken").to("token")
        setDefaultNationalPracticeCodeSessionVariable(gpSystem)
    }

    @Given("I have no IM1 Connection Token for (.*)")
    fun givenIHaveNoIm1ConnectionToken(gpSystem: String) {
        setSessionVariable("ConnectionToken").to(null)
        setDefaultNationalPracticeCodeSessionVariable(gpSystem)
    }

    private fun setDefaultNationalPracticeCodeSessionVariable(gpSystem: String) {
        val odsCode = PatientVerificationFactory.getForSupplier(gpSystem).odsCode
        setSessionVariable("NationalPracticeCode").to(odsCode)
    }

    @Given("Vision responds with a security header error")
    fun visionRespondsWithASecurityHeaderError() {
        setSessionVariable("ConnectionToken").to(VisionMockDefaults.patientVision.connectionToken)
        setSessionVariable("NationalPracticeCode").to(VisionMockDefaults.DEFAULT_ODS_CODE_VISION)
        mockingClient
                .forVision {
                    authentication.getConfigurationRequest(
                            visionUserSession = VisionUserSession.fromPatient(Patient.getDefault("VISION")))
                            .respondWithSecurityHeaderError()
                }
    }

    @Given("Vision responds with an invalid request error")
    fun visionRespondsWithAInvalidRequestError() {
        setSessionVariable("ConnectionToken").to(VisionMockDefaults.patientVision.connectionToken)
        setSessionVariable("NationalPracticeCode").to(VisionMockDefaults.DEFAULT_ODS_CODE_VISION)

        mockingClient
                .forVision {
                    authentication.getConfigurationRequest(
                            visionUserSession = VisionUserSession.fromPatient(Patient.getDefault("VISION")))
                            .respondWithInvalidRequest()
                }
    }

    @Given("Vision responds with an unknown error")
    fun visionRespondsWithAnUnknownError() {
        val patient =  Patient.getDefault("VISION")
        setSessionVariable("ConnectionToken").to(patient.connectionToken)
        setSessionVariable("NationalPracticeCode").to(patient.odsCode)
        mockingClient
                .forVision {
                    authentication.getConfigurationRequest(
                            VisionMockDefaults.getVisionUserSession(patient))
                            .respondWithUnknownError()
                }
    }

    @Given("I have an (.*) ODS Code not in expected format")
    fun givenIHaveAnOdsCodeNotInExpectedFormat(gpSystem: String) {
        setSessionVariable("NationalPracticeCode").to("£$*&")
        setSessionVariable("ConnectionToken").to(Patient.getDefault(gpSystem).connectionToken)
    }

    @Given("I have an (.*) ODS Code that does not exists")
    fun givenIHaveAnOdsCodeThatDoesNotExists(gpSystem: String) {
        setSessionVariable("NationalPracticeCode").to("E99999")
        setSessionVariable("ConnectionToken").to(Patient.getDefault(gpSystem).connectionToken)
    }

    @Given("I have no (.*) ODS Code")
    fun givenIHaveNoOdsCode(gpSystem: String) {
        setSessionVariable("NationalPracticeCode").to(null)
        setSessionVariable("ConnectionToken").to(Patient.getDefault(gpSystem).connectionToken)
    }

    @Given("I have valid credentials for a (.*) patient with one NHS Number")
    fun givenIHaveValidCredentialsForAPatientWithOneNhsNumber(gpSystem: String) {
        PatientVerificationFactory.getForSupplier(gpSystem).validPatientWithOneNhsNumber()
    }

    @Given("I have valid credentials for a (.*) patient with multiple NHS Numbers")
    fun givenIHaveValidCredentialsForAPatientWithMultipleNhsNumbers(gpSystem: String) {
        PatientVerificationFactory.getForSupplier(gpSystem).validPatientWithMultipleNumbers()
    }


    @Given("I have valid credentials for a (.*) patient with no NHS Number")
    fun givenIHaveValidCredentialsForAPatientWithNoNhsNumber(gpSystem: String) {

        PatientVerificationFactory.getForSupplier(gpSystem).validPatientWithNoNhsNumber()
    }

    @When("I verify patient data")
    fun whenIVerifyPatientData() {
        val connectionToken = sessionVariableCalled<String>("ConnectionToken")

        val odsCode = sessionVariableCalled<String>("NationalPracticeCode")

        try {
            val result = sessionVariableCalled<WorkerClient>(WorkerClient::class)
                    .authentication.getIm1Connection(connectionToken, odsCode)
            setSessionVariable(Im1ConnectionResponse::class).to(result)
        } catch (httpException: NhsoHttpException) {
            setSessionVariable("HttpException").to(httpException)
        }
    }

    @Then("I receive the expected NHS Numbers?$")
    fun thenIReceiveTheExpectedNhsNumber() {
        val result = sessionVariableCalled<Im1ConnectionResponse>(Im1ConnectionResponse::class)
        val nhsNumbers = sessionVariableCalled<Array<PatientIdentifier>>("NhsNumbers")
        val connectionToken = sessionVariableCalled<String>("ConnectionToken")
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

    @Then("I receive no NHS Number")
    fun thenIReceiveNoNhsNumber() {
        val result = sessionVariableCalled<Im1ConnectionResponse>(Im1ConnectionResponse::class)
        Assert.assertNotNull("IM1 connection response expected, but was null", result)
        Assert.assertEquals(0, result.nhsNumbers!!.count())
    }
}
