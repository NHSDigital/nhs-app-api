package features.sharedStepDefinitions.backend

import cucumber.api.java.en.Given
import cucumber.api.java.en.Then
import cucumber.api.java.en.When
import mocking.defaults.MockDefaults
import mocking.defaults.TppMockDefaults
import mocking.emis.demographics.PatientIdentifier
import mocking.emis.demographics.Sex
import mocking.emis.models.AssociationType
import mocking.emis.models.IdentifierType
import mocking.tpp.models.*
import mocking.vision.models.*
import models.Patient
import net.serenitybdd.core.Serenity.*

import org.junit.Assert
import worker.NhsoHttpException
import worker.WorkerClient
import worker.models.patient.Im1ConnectionResponse

class PatientVerificationSteps : AbstractSteps() {

    private val EMIS = "EMIS"
    private val TPP = "TPP"
    private val VISION = "VISION"

    @Given("I have an (.*) IM1 Connection Token that does not exist")
    fun givenIHaveAnImConnectionTokenThatDoesNotExist(gpSystem: String) {
        when (gpSystem) {
            TPP -> {
                val nonExistingConnectionToken = "{\"accountid\":\"999999999\",\"passphrase\":\"nonexistingpassword\"}"

                val tppNonExistingAccountIdErrorResponse = Error(
                        errorCode = "9",
                        userFriendlyMessage = "There was a problem logging on",
                        uuid = "47788ae4-10e9-4f2c-9043-e08d285b67b6"
                )

                mockingClient.forTpp { authentication.authenticateRequest(TppMockDefaults.tppAuthenticateRequest)
                        .respondWithError(tppNonExistingAccountIdErrorResponse) }

                setSessionVariable("NationalPracticeCode").to(TppMockDefaults.DEFAULT_ODS_CODE_TPP)
                setSessionVariable("ConnectionToken").to(nonExistingConnectionToken)
            }
            EMIS -> {
                val nonExistingConnectionToken = "0d135b66-a8b0-46b2-b437-cfe75edc773d"
                val patient = Patient(
                        connectionToken = nonExistingConnectionToken,
                        odsCode = MockDefaults.DEFAULT_ODS_CODE,
                        endUserSessionId = "zVGrzHH7YUPeEBRk1nat1D"
                )

                mockingClient.forEmis { authentication.sessionRequest(patient).respondWithUserNotRegistered() }
                mockingClient.forEmis { authentication.endUserSessionRequest().respondWithSuccess(patient.endUserSessionId) }
                mockingClient.forEmis {
                    myRecord.demographicsRequest(patient).respondWithSuccess(patient,
                            patientIdentifiers = arrayOf(
                                    PatientIdentifier(
                                            identifierType = IdentifierType.NhsNumber,
                                            identifierValue = MockDefaults.patient.nhsNumbers[0])))
                }
                setSessionVariable("ConnectionToken").to(nonExistingConnectionToken)
                setSessionVariable("NationalPracticeCode").to(patient.odsCode)
            }
            VISION -> {
                val patient = MockDefaults.patientVision
                var nonExistingConnectionToken = "{\"rosuAccountid\":\"999999999\",\"apiKey\":\"nonexistingapikey\"}"

                mockingClient
                        .forVision {
                            getConfigurationRequest(
                                    visionUserSession = VisionUserSession(
                                            "999999999",
                                            "nonexistingapikey",
                                            Patient.aderynCanon.odsCode, patientId =  Patient.aderynCanon.patientId))
                                    .respondWitInvalidUserCredentials()
                        }
                setSessionVariable("ConnectionToken").to(nonExistingConnectionToken)
                setSessionVariable("NationalPracticeCode").to(patient.odsCode)
                setSessionVariable("NhsNumber").to(patient.nhsNumbers[0])
            }
        }
    }

    @Given("I have an (.*) IM1 Connection Token that is in an invalid format")
    fun givenIHaveAnIm1ConnectionTokenThatIsInAnInvalidFormat(gpSystem: String) {
        setSessionVariable("ConnectionToken").to("token")
        setDefaultNationalPracticeCodeSessionVariable(gpSystem)
    }

    @Given("Vision responds with a security header error")
    fun visionRespondsWithASecurityHeaderError() {
        setSessionVariable("ConnectionToken").to(MockDefaults.patientVision.connectionToken)
        setSessionVariable("NationalPracticeCode").to(MockDefaults.DEFAULT_ODS_CODE_VISION)

        mockingClient
                .forVision {
                    getConfigurationRequest(
                            visionUserSession = VisionUserSession(
                                    Patient.aderynCanon.rosuAccountId,
                                    Patient.aderynCanon.apiKey,
                                    Patient.aderynCanon.odsCode,
                                    Patient.aderynCanon.patientId))
                            .respondWithSecurityHeaderError()
                }
    }

    @Given("Vision responds with an invalid request error")
    fun visionRespondsWithAInvalidRequestError() {
        setSessionVariable("ConnectionToken").to(MockDefaults.patientVision.connectionToken)
        setSessionVariable("NationalPracticeCode").to(MockDefaults.DEFAULT_ODS_CODE_VISION)

        mockingClient
                .forVision {
                    getConfigurationRequest(
                            visionUserSession = VisionUserSession(
                                    Patient.aderynCanon.rosuAccountId,
                                    Patient.aderynCanon.apiKey,
                                    Patient.aderynCanon.odsCode,
                                    Patient.aderynCanon.patientId))
                            .respondWithInvalidRequest()
                }
    }

    @Given("Vision responds with an unknown error")
    fun visionRespondsWithAnUnknownError() {
        setSessionVariable("ConnectionToken").to(MockDefaults.patientVision.connectionToken)
        setSessionVariable("NationalPracticeCode").to(MockDefaults.patientVision.odsCode)
        mockingClient
                .forVision {
                    getConfigurationRequest(
                            MockDefaults.visionUserSession)
                            .respondWithUnknownError()
                }
    }

    @Given("I have no IM1 Connection Token for (.*)")
    fun givenIHaveNoIm1ConnectionToken(gpSystem: String) {
        setSessionVariable("ConnectionToken").to(null)
        setDefaultNationalPracticeCodeSessionVariable(gpSystem)
    }

    private fun setDefaultNationalPracticeCodeSessionVariable(gpSystem: String) {
        when (gpSystem) {
            TPP -> {
                setSessionVariable("NationalPracticeCode").to(TppMockDefaults.DEFAULT_ODS_CODE_TPP)
            }
            EMIS -> {
                setSessionVariable("NationalPracticeCode").to(MockDefaults.DEFAULT_ODS_CODE)
            }
            VISION -> {
                setSessionVariable("NationalPracticeCode").to(MockDefaults.DEFAULT_ODS_CODE_VISION)
            }
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
        when (gpSystem) {
            TPP -> {
                val patient = Patient(
                        title = "Mr",
                        firstName = "Kevin",
                        surname = "Barry",
                        connectionToken = "{\"accountId\": \"520993083\", \"passphrase\":\"c2axhQ9VWB2/62XFxvKrNKh9JwgLk0NFY15hIdI6aRytptqiBs6r/k+0OvGEZfcEdMLJEMp/J4pkOGm2ViaSLca49ODQzz4y+Cu2xOxLaehq/SjEIwflsWeSwCvCAxroId1bXejTdNsV17fOAD0M5nAZF6X9TysOfRR/j5tuR+o=\"}",
                        odsCode = TppMockDefaults.DEFAULT_ODS_CODE_TPP,
                        endUserSessionId = MockDefaults.DEFAULT_END_USER_SESSION_ID,
                        nhsNumbers = listOf("5785445875"),
                        accountId = "520993083",
                        passphrase = "c2axhQ9VWB2/62XFxvKrNKh9JwgLk0NFY15hIdI6aRytptqiBs6r/k+0OvGEZfcEdMLJEMp/J4pkOGm2ViaSLca49ODQzz4y+Cu2xOxLaehq/SjEIwflsWeSwCvCAxroId1bXejTdNsV17fOAD0M5nAZF6X9TysOfRR/j5tuR+o=",
                        patientId = "84df400000000000",
                        onlineUserId = "84df400000000000",
                        dateOfBirth = "1985-05-29T00:00:00.0Z",
                        sex = Sex.Male
                )

                val authenticateRequest = Authenticate(
                        apiVersion = "1",
                        accountId = patient.accountId,
                        passphrase = patient.passphrase,
                        unitId = "KGPD",
                        uuid = "af0a8175-e6c2-4c49-883e-020b2b3600f9",
                        application = Application(
                                name = "NhsApp",
                                version = "1.0",
                                providerId = "b891fc3b51d5e7c1",
                                deviceType = "NhsApp"
                        )
                )

                val tppAuthenticateReplyResponse = AuthenticateReply(
                        patientId = patient.patientId,
                        onlineUserId = patient.onlineUserId,
                        uuid = "af0a8175-e6c2-4c49-883e-020b2b3600f9",
                        user = User(
                                person = Person(
                                        patientId = patient.patientId,
                                        dateOfBirth = patient.dateOfBirth,
                                        gender = patient.sex.name,
                                        nationalId = NationalId(
                                                type = "NHS",
                                                value = patient.nhsNumbers.first()
                                        ),
                                        personName = PersonName(
                                                name = "${patient.title} ${patient.firstName} ${patient.surname}"
                                        )
                                )
                        )
                )

                mockingClient.forTpp { authentication.authenticateRequest(authenticateRequest).respondWithSuccess(tppAuthenticateReplyResponse) }

                setSessionVariable("ConnectionToken").to(patient.connectionToken)
                setSessionVariable("NationalPracticeCode").to(patient.odsCode)
                setSessionVariable("NhsNumbers").to(arrayOf(PatientIdentifier(patient.nhsNumbers[0], IdentifierType.NhsNumber)))

            }
            EMIS -> {
                emisValidCredentialsWithNHSNumbers(listOf("NHS_number"))
            }
            VISION -> {
                val patient = MockDefaults.patientVision
                visionValidCredentialsWithNHSNumbers(arrayOf(patient.nhsNumbers[0]))
            }
        }
    }

    @Given("I have valid credentials for a (.*) patient with multiple NHS Numbers")
    fun givenIHaveValidCredentialsForAPatientWithMultipleNhsNumbers(gpSystem: String) {
        when (gpSystem) {
            EMIS -> {
                emisValidCredentialsWithNHSNumbers(listOf("NHS_number1", "NHS_number2"))
            }
            VISION -> {
                val patient = MockDefaults.patientVision
                val nhsNumbers = arrayOf(patient.nhsNumbers[0], "5785445866")
                visionValidCredentialsWithNHSNumbers(nhsNumbers)
            }
        }
    }

    private fun emisValidCredentialsWithNHSNumbers(numbers: List<String>) {
        val patient = Patient(
                title = "Miss",
                firstName = "Alexia",
                surname = "Scott",
                odsCode = MockDefaults.DEFAULT_ODS_CODE,
                connectionToken = "fe81f191-b016-466e-aeb2-64f08f2330a4",
                sessionId = "xkWiivK1WBAkxIN9CDrGyy",
                endUserSessionId = "9RFDWiqTO8zBWrp2p8s4K7",
                userPatientLinkToken = "KxLiDl5nRS60DzIlrKoFSl",
                nhsNumbers = numbers,
                dateOfBirth = "1985-05-29T00:00:00.0Z")

        val nhsNumbers = numbers.map { number -> PatientIdentifier(number, identifierType = IdentifierType.NhsNumber) }.toTypedArray()

        mockingClient.forEmis { authentication.endUserSessionRequest().respondWithSuccess(patient.endUserSessionId) }
        mockingClient.forEmis { authentication.sessionRequest(patient).respondWithSuccess(patient, AssociationType.Self) }
        mockingClient.forEmis { myRecord.demographicsRequest(patient).respondWithSuccess(patient, nhsNumbers) }

        setSessionVariable("ConnectionToken").to(patient.connectionToken)
        setSessionVariable("NationalPracticeCode").to(patient.odsCode)
        setSessionVariable("NhsNumbers").to(nhsNumbers)

    }

    private fun visionValidCredentialsWithNHSNumbers(nhsNumbers: Array<String>) {
        val patient = MockDefaults.patientVision
        mockingClient
                .forVision {
                    getConfigurationRequest(
                            visionUserSession = VisionUserSession(
                                    Patient.aderynCanon.rosuAccountId,
                                    Patient.aderynCanon.apiKey,
                                    Patient.aderynCanon.odsCode,
                                    Patient.aderynCanon.patientId))
                            .respondWithSuccess(configuration = Configuration(
                                    account = Account(patient.patientId,
                                            patientNumber = nhsNumbers.map { number -> PatientNumber(number = number) },
                                            name = MockDefaults.getFullPatientName(patient)
                                    )))
                }
        setSessionVariable("ConnectionToken").to(patient.connectionToken)
        setSessionVariable("NationalPracticeCode").to(patient.odsCode)
        setSessionVariable("NhsNumbers").to(nhsNumbers.map { number -> PatientIdentifier(number, IdentifierType.NhsNumber) }.toTypedArray())
    }

    @Given("I have valid credentials for a (.*) patient with no NHS Number")
    fun givenIHaveValidCredentialsForAPatientWithNoNhsNumber(gpSystem: String) {
        when (gpSystem) {
            EMIS -> {
                emisValidCredentialsWithNHSNumbers(arrayListOf())
            }
            VISION -> {
                val patient = MockDefaults.patientVision

                mockingClient
                        .forVision {
                            getConfigurationRequest(
                                    visionUserSession = VisionUserSession(
                                            Patient.aderynCanon.rosuAccountId,
                                            Patient.aderynCanon.apiKey,
                                            Patient.aderynCanon.odsCode,
                                            Patient.aderynCanon.patientId))
                                    .respondWithSuccess(configuration = Configuration(account = Account(patient.patientId,
                                            patientNumber = null, name = MockDefaults.getFullPatientName(patient))
                                    ))
                        }
                setSessionVariable("ConnectionToken").to(patient.connectionToken)
                setSessionVariable("NationalPracticeCode").to(patient.odsCode)
                setSessionVariable("NhsNumber").to("")
            }
        }
    }

    @When("I verify patient data")
    fun whenIVerifyPatientData() {
        val connectionToken = sessionVariableCalled<String>("ConnectionToken")
        val odsCode = sessionVariableCalled<String>("NationalPracticeCode")

        try {
            val result = sessionVariableCalled<WorkerClient>(WorkerClient::class).authentication.getIm1Connection(connectionToken, odsCode)
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
        val resultNhsNumbers = result.nhsNumbers!!.map { number -> number.nhsNumber }.toTypedArray()
        val expectedNhsNumbers = nhsNumbers.map { number -> formatNhsNumber(number.identifierValue!!) }.toTypedArray()
        Assert.assertEquals(resultNhsNumbers.count(), nhsNumbers.count())
        Assert.assertArrayEquals("Expected NHS Numbers", expectedNhsNumbers, resultNhsNumbers)
        Assert.assertEquals(result.connectionToken, connectionToken)
    }

    @Then("I receive no NHS Number")
    fun thenIReceiveNoNhsNumber() {
        val result = sessionVariableCalled<Im1ConnectionResponse>(Im1ConnectionResponse::class)
        Assert.assertNotNull("IM1 connection response expected, but was null", result)
        Assert.assertEquals(result.nhsNumbers!!.count(), 0)
    }

    private fun formatNhsNumber(nhsNumber: String): String {
        if (nhsNumber.isNullOrEmpty()) return ""

        if (nhsNumber.length != 10) return nhsNumber

        return String.format("%s %s %s",
                nhsNumber.substring(0, 3),
                nhsNumber.substring(3, 6),
                nhsNumber.substring(6, 10))
    }
}