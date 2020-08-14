package features.authentication.factories

import constants.ErrorResponseCodeTpp
import constants.Supplier
import features.authentication.stepDefinitions.PatientVerificationSerenityHelpers
import mocking.data.myrecord.DemographicsData
import mocking.defaults.TppMockDefaults
import mocking.emis.demographics.PatientIdentifier
import mocking.emis.models.IdentifierType
import mocking.tpp.models.Application
import mocking.tpp.models.Authenticate
import mocking.tpp.models.AuthenticateReply
import mocking.tpp.models.Error
import mocking.tpp.models.NationalId
import mocking.tpp.models.Person
import mocking.tpp.models.PersonName
import mocking.tpp.models.User
import models.Patient
import org.junit.Assert
import utils.SerenityHelpers
import utils.set
import java.time.Duration

private const val REQUEST_DELAY = 1000_000L

class PatientVerificationFactoryTpp: PatientVerificationFactory(Supplier.TPP){
    override fun validPatientWithNoNhsNumber() {
        Assert.fail("TPP user cannot have no NHS Number")
    }

    override fun validPatientWithMultipleNumbers() {
        Assert.fail("TPP user cannot have multiple NHS Numbers")
    }

    override fun validPatientWithOneNhsNumber() {
        val patient = Patient.getDefault(Supplier.TPP).copy(nhsNumbers = listOf("5785445875"))

        val authenticateRequest = Authenticate(
                apiVersion = "1",
                accountId = patient.accountId,
                passphrase = patient.passphrase,
                unitId = "A82648",
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
                                dateOfBirth = patient.age.dateOfBirth,
                                gender = patient.sex.name,
                                nationalId = NationalId(
                                        type = "NHS",
                                        value = patient.nhsNumbers.first()
                                ),
                                personName = PersonName(patient.formattedFullName())
                        )
                ),
                person =  mutableListOf()
        )

        mockingClient.forTpp.mock { authentication.authenticateRequest(authenticateRequest)
                .respondWithSuccess(tppAuthenticateReplyResponse) }


        PatientVerificationSerenityHelpers.ConnectionToken.set(patient.connectionToken)
        PatientVerificationSerenityHelpers.NationalPracticeCode.set(patient.odsCode)
        PatientVerificationSerenityHelpers.NhsNumbers.set(
                arrayOf(PatientIdentifier(patient.nhsNumbers.first(),
                IdentifierType.NhsNumber)))

    }

    override val odsCode: String = TppMockDefaults.DEFAULT_ODS_CODE_TPP

    override fun im1ConnectionTokenDoesNotExist() {

        val nonExistingConnectionToken = createConnectionToken("9999999999", "nonExising")

        val tppNonExistingAccountIdErrorResponse = Error(
                errorCode = "9",
                userFriendlyMessage = "There was a problem logging on",
                uuid = "47788ae4-10e9-4f2c-9043-e08d285b67b6"
        )

        mockingClient.forTpp.mock {
            authentication.authenticateRequest(
                    TppMockDefaults.tppAuthenticateRequest(SerenityHelpers.getPatient()))
                .respondWithError(tppNonExistingAccountIdErrorResponse) }

        PatientVerificationSerenityHelpers.ConnectionToken.set(nonExistingConnectionToken)
        PatientVerificationSerenityHelpers.NationalPracticeCode.set(TppMockDefaults.DEFAULT_ODS_CODE_TPP)
    }

    override fun connectionToExternalServiceFailed() {
        throw NotImplementedError("Not implemented for this GP system")
    }

    fun createConnectionToken(accountId: String = TppMockDefaults.DEFAULT_TPP_ACCOUNT_ID,
                              passphrase : String = TppMockDefaults.DEFAULT_TPP_PASSPHRASE,
                              providerId :String = TppMockDefaults.DEFAULT_TPP_PROVIDER_ID): String {
        return "{\"AccountId\":\"$accountId\"" +
                ",\"Passphrase\":\"$passphrase\"" +
                ",\"ProviderId\":\"$providerId\"}"
    }

    override fun setSessionExtendMockResponse(patient: Patient, expectedResponse: String) {
        when (expectedResponse) {
            "Success" -> {
                mockingClient.forTpp.mock {
                    authentication.patientSelectedPost(patient.tppUserSession!!)
                            .respondWithSuccess(DemographicsData.getTppDemographicsData(patient))
                }
            }
            "bad gateway" -> {
                mockingClient.forTpp.mock {
                    authentication.patientSelectedPost(patient.tppUserSession!!)
                            .respondWithError(Error(ErrorResponseCodeTpp.UNKNOWN_ERROR,
                                    "Error occurred when trying to extend session",
                                    "1f907c07-9063-4d3a-81d7-ee8c98c54f4a"))
                }
            }
            "gateway timeout" -> {
                mockingClient.forTpp.mock {
                    authentication.patientSelectedPost(patient.tppUserSession!!)
                            .respondWithSuccess(DemographicsData.getTppDemographicsData(patient))
                            .delayedBy(Duration.ofSeconds(REQUEST_DELAY))

                }
            }
            "unauthorized" -> {
                mockingClient.forTpp.mock {
                    authentication.patientSelectedPost(patient.tppUserSession!!)
                            .respondWithError(Error(ErrorResponseCodeTpp.NO_ACCESS,
                                    "Not Authorized to return record",
                                    "1f907c07-9063-4d3a-81d7-ee8c98c54f4a"))
                }
            }
        }
    }

    override fun oldOdsCodeAndConnectionTokenForPatientThatHasSinceMovedToADifferentPractice() {
        throw NotImplementedError("Not implemented for this GP system")
    }

    override fun gpSystemNotAvailable() {
        val patient = SerenityHelpers.getPatient()
        mockingClient.forTpp.mock {
            authentication.authenticateRequest(Authenticate())
                    .respondWithServiceUnavailable()
        }

        PatientVerificationSerenityHelpers.ConnectionToken.set(patient.connectionToken)
        PatientVerificationSerenityHelpers.NationalPracticeCode.set(patient.odsCode)
    }
}
