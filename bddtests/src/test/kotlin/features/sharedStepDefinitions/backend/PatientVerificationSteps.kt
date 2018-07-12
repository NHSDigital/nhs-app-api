package features.sharedStepDefinitions.backend

import cucumber.api.java.en.Given
import cucumber.api.java.en.Then
import cucumber.api.java.en.When
import mocking.defaults.MockDefaults
import mocking.emis.demographics.PatientIdentifier
import mocking.emis.demographics.Sex
import mocking.emis.models.AssociationType
import mocking.emis.models.IdentifierType
import mocking.tpp.models.*
import models.Patient
import net.serenitybdd.core.Serenity.sessionVariableCalled
import net.serenitybdd.core.Serenity.setSessionVariable

import org.junit.Assert
import worker.NhsoHttpException
import worker.WorkerClient
import worker.models.patient.Im1ConnectionResponse


class PatientVerificationSteps : AbstractSteps() {

    private val defaultOdsCode = "E87649"
    private val defaultConnectionToken = "bce74b97-4296-414a-a4f5-0f1bf5732ba6"
    private val EMIS = "EMIS"
    private val TPP = "TPP"

    @Given("I have an (.*) IM1 Connection Token that does not exist")
    fun givenIHaveAnImConnectionTokenThatDoesNotExist(gpSystem: String) {
        when (gpSystem) {
            TPP -> {
                var nonExistingConnectionToken = "{\"accountid\":\"999999999\",\"passphrase\":\"nonexistingpassword\"}"
                mockingClient.forTpp { authenticateRequest(MockDefaults.tppAuthenticateRequest).respondWithError(MockDefaults.tppNonExistingAccountIdErrorResponse) }

                setSessionVariable("NationalPracticeCode").to(MockDefaults.DEFAULT_ODS_CODE_TPP)
                setSessionVariable("ConnectionToken").to(nonExistingConnectionToken)
            }
            EMIS -> {
                val nonExistingConnectionToken = "0d135b66-a8b0-46b2-b437-cfe75edc773d"
                val patient = Patient(
                        connectionToken = nonExistingConnectionToken,
                        odsCode = defaultOdsCode,
                        endUserSessionId = "zVGrzHH7YUPeEBRk1nat1D"
                )

                mockingClient.forEmis { sessionRequest(patient).respondWithUserNotRegistered() }
                mockingClient.forEmis { endUserSessionRequest().respondWithSuccess(patient.endUserSessionId) }
                mockingClient.forEmis {
                    demographicsRequest(patient).respondWithSuccess(patient,
                            patientIdentifiers = arrayOf(
                                    PatientIdentifier(
                                            identifierType = IdentifierType.NhsNumber,
                                            identifierValue = MockDefaults.patient.nhsNumbers[0])))
                }
                setSessionVariable("ConnectionToken").to(nonExistingConnectionToken)
                setSessionVariable("NationalPracticeCode").to(patient.odsCode)
            }
        }
    }

    @Given("I have an (.*) IM1 Connection Token that is in an invalid format")
    fun givenIHaveAnIm1ConnectionTokenThatIsInAnInvalidFormat(gpSystem: String) {
        setSessionVariable("ConnectionToken").to("token")
        when (gpSystem) {
            TPP -> {
                setSessionVariable("NationalPracticeCode").to(MockDefaults.DEFAULT_ODS_CODE_TPP)
            }
            EMIS -> {
                setSessionVariable("NationalPracticeCode").to(defaultOdsCode)
            }
        }
    }

    @Given("I have no IM1 Connection Token for (.*)")
    fun givenIHaveNoIm1ConnectionToken(gpSystem: String) {
        setSessionVariable("ConnectionToken").to(null)
        when (gpSystem) {
            TPP -> {
                setSessionVariable("NationalPracticeCode").to(MockDefaults.DEFAULT_ODS_CODE_TPP)
            }
            EMIS -> {
                setSessionVariable("NationalPracticeCode").to(defaultOdsCode)
            }
        }
    }

    @Given("I have an (.*) ODS Code not in expected format")
    fun givenIHaveAnOdsCodeNotInExpectedFormat(gpSystem: String) {
        setSessionVariable("NationalPracticeCode").to("£$*&")
        when (gpSystem) {
            TPP -> {
                setSessionVariable("ConnectionToken").to(MockDefaults.patientTpp.connectionToken)
            }
            EMIS -> {
                setSessionVariable("ConnectionToken").to(defaultConnectionToken)
            }
        }
    }

    @Given("I have an (.*) ODS Code that does not exists")
    fun givenIHaveAnOdsCodeThatDoesNotExists(gpSystem: String) {
        setSessionVariable("NationalPracticeCode").to("E99999")
        when (gpSystem) {
            TPP -> {
                setSessionVariable("ConnectionToken").to(MockDefaults.patientTpp.connectionToken)
            }
            EMIS -> {
                setSessionVariable("ConnectionToken").to(defaultConnectionToken)
            }
        }
    }

    @Given("I have no (.*) ODS Code")
    fun givenIHaveNoOdsCode(gpSystem: String) {
        setSessionVariable("NationalPracticeCode").to(null)
        when (gpSystem) {
            TPP -> {
                setSessionVariable("ConnectionToken").to(MockDefaults.patientTpp.connectionToken)
            }
            EMIS -> {
                setSessionVariable("ConnectionToken").to(defaultConnectionToken)
            }
        }
    }

    @Given("I have valid credentials for a (.*) patient with one NHS Number")
    fun givenIHaveValidCredentialsForAPatientWithOneNhsNumber(gpSystem: String) {
        when (gpSystem) {
            TPP -> {
                val patient = Patient(
                        title =  "Mr",
                        firstName =  "Kevin",
                        surname =  "Barry",
                        connectionToken =  "{\"accountId\": \"520993083\", \"passphrase\":\"c2axhQ9VWB2/62XFxvKrNKh9JwgLk0NFY15hIdI6aRytptqiBs6r/k+0OvGEZfcEdMLJEMp/J4pkOGm2ViaSLca49ODQzz4y+Cu2xOxLaehq/SjEIwflsWeSwCvCAxroId1bXejTdNsV17fOAD0M5nAZF6X9TysOfRR/j5tuR+o=\"}",
                        odsCode =  MockDefaults.DEFAULT_ODS_CODE_TPP,
                        endUserSessionId =  MockDefaults.DEFAULT_END_USER_SESSION_ID,
                        nhsNumbers =  listOf("5785445875"),
                        accountId =  "520993083",
                        passphrase = "c2axhQ9VWB2/62XFxvKrNKh9JwgLk0NFY15hIdI6aRytptqiBs6r/k+0OvGEZfcEdMLJEMp/J4pkOGm2ViaSLca49ODQzz4y+Cu2xOxLaehq/SjEIwflsWeSwCvCAxroId1bXejTdNsV17fOAD0M5nAZF6X9TysOfRR/j5tuR+o=",
                        patientId = "84df400000000000",
                        onlineUserId = "84df400000000000",
                        dateOfBirth =  "1985-05-29T00:00:00.0Z",
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

                mockingClient.forTpp { authenticateRequest(authenticateRequest).respondWithSuccess(tppAuthenticateReplyResponse) }

                setSessionVariable("ConnectionToken").to(patient.connectionToken)
                setSessionVariable("NationalPracticeCode").to(patient.odsCode)
                setSessionVariable("NhsNumber").to(patient.nhsNumbers[0])

            }
            EMIS -> {
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

                mockingClient.forEmis { demographicsRequest(patient).respondWithSuccess(patient, arrayOf(PatientIdentifier(patient.nhsNumbers[0], identifierType = IdentifierType.NhsNumber))) }
                mockingClient.forEmis { endUserSessionRequest().respondWithSuccess(patient.endUserSessionId) }
                mockingClient.forEmis { sessionRequest(patient).respondWithSuccess(patient, AssociationType.Self) }

                setSessionVariable("ConnectionToken").to(patient.connectionToken)
                setSessionVariable("NationalPracticeCode").to(patient.odsCode)
                setSessionVariable("NhsNumber").to(patient.nhsNumbers[0])
            }
        }
    }

    @Given("I have valid credentials for a (.*) patient with multiple NHS Numbers")
    fun givenIHaveValidCredentialsForAPatientWithMultipleNhsNumbers(gpSystem: String) {
        when (gpSystem) {
            EMIS -> {
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
                val nhsNumbers = arrayOf(PatientIdentifier(patient.nhsNumbers[0], identifierType = IdentifierType.NhsNumber), PatientIdentifier(patient.nhsNumbers[1], identifierType = IdentifierType.NhsNumber))

                mockingClient.forEmis { endUserSessionRequest().respondWithSuccess(patient.endUserSessionId) }
                mockingClient.forEmis { sessionRequest(patient).respondWithSuccess(patient, AssociationType.Self) }
                mockingClient.forEmis { demographicsRequest(patient).respondWithSuccess(patient, nhsNumbers) }

                setSessionVariable("ConnectionToken").to(patient.connectionToken)
                setSessionVariable("NationalPracticeCode").to(patient.odsCode)
                setSessionVariable("NhsNumbers").to(nhsNumbers)
            }
        }
    }

    @Given("I have valid credentials for a (.*) patient with no NHS Number")
    fun givenIHaveValidCredentialsForAPatientWithNoNhsNumber(gpSystem: String) {
        when (gpSystem) {
            EMIS -> {
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
        }
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
        Assert.assertEquals(result.nhsNumbers!![0].nhsNumber, formatNhsNumber(nhsNumber))
        Assert.assertEquals(result.connectionToken, connectionToken)
    }

    @Then("I receive the expected NHS Numbers")
    fun thenIReceiveTheExpectedNhsNumbers() {
        val result = sessionVariableCalled<Im1ConnectionResponse>(Im1ConnectionResponse::class)
        val nhsNumbers = sessionVariableCalled<Array<PatientIdentifier>>("NhsNumbers")
        val connectionToken = sessionVariableCalled<String>("ConnectionToken")
        Assert.assertNotNull(result)
        Assert.assertEquals(result.nhsNumbers!!.count(), nhsNumbers.count())
        Assert.assertEquals(result.nhsNumbers!![0].nhsNumber, formatNhsNumber(nhsNumbers[0].identifierValue!!))
        Assert.assertEquals(result.nhsNumbers!![1].nhsNumber, formatNhsNumber(nhsNumbers[1].identifierValue!!))
        Assert.assertEquals(result.connectionToken, connectionToken)
    }

    @Then("I receive no NHS Number")
    fun thenIReceiveNoNhsNumber() {
        val result = sessionVariableCalled<Im1ConnectionResponse>(Im1ConnectionResponse::class)
        Assert.assertEquals(result.nhsNumbers!!.count(), 0)
    }

    private fun formatNhsNumber(nhsNumber: String) : String {
        if (nhsNumber.isNullOrEmpty()) return ""

        if (nhsNumber.length != 10) return nhsNumber

        return String.format("%s %s %s",
                nhsNumber.substring(0, 3),
                nhsNumber.substring(3, 6),
                nhsNumber.substring(6, 10))
    }
}