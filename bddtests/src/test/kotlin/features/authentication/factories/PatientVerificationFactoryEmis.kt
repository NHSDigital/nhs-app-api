package features.authentication.factories

import constants.Supplier
import features.authentication.stepDefinitions.PatientVerificationSerenityHelpers
import mocking.defaults.EmisMockDefaults
import mocking.emis.demographics.PatientIdentifier
import mocking.emis.models.AssociationType
import mocking.emis.models.IdentifierType
import models.Patient
import models.PatientAge
import models.PatientName
import org.apache.http.HttpStatus
import utils.set
import java.time.Duration

private const val REQUEST_DELAY = 1000_000L

class PatientVerificationFactoryEmis: PatientVerificationFactory(Supplier.EMIS) {

    override fun validPatientWithNoNhsNumber() {
        emisValidCredentialsWithNHSNumbers(arrayListOf())
    }

    override fun validPatientWithMultipleNumbers() {
        emisValidCredentialsWithNHSNumbers(listOf("NHS_number1", "NHS_number2"))
    }

    override fun validPatientWithOneNhsNumber() {
        emisValidCredentialsWithNHSNumbers(listOf("NHS_number"))
    }


    override val odsCode: String = EmisMockDefaults.DEFAULT_ODS_CODE_EMIS

    override fun im1ConnectionTokenDoesNotExist() {
        val nonExistingConnectionToken = "0d135b66-a8b0-46b2-b437-cfe75edc773d"
        val patient = Patient(
                connectionToken = nonExistingConnectionToken,
                odsCode = EmisMockDefaults.DEFAULT_ODS_CODE_EMIS,
                endUserSessionId = "zVGrzHH7YUPeEBRk1nat1D"
        )

        mockingClient.forEmis.mock { authentication.sessionRequest(patient).respondWithUserNotRegistered() }
        mockingClient.forEmis.mock { authentication.endUserSessionRequest()
                .respondWithSuccess(patient.endUserSessionId) }
        mockingClient.forEmis.mock {
            myRecord.demographicsRequest(patient).respondWithSuccess(patient,
                    patientIdentifiers = arrayOf(
                            PatientIdentifier(
                                    identifierType = IdentifierType.NhsNumber,
                                    identifierValue = EmisMockDefaults.patientEmis.nhsNumbers.first())))
        }

        PatientVerificationSerenityHelpers.ConnectionToken.set(nonExistingConnectionToken)
        PatientVerificationSerenityHelpers.NationalPracticeCode.set(patient.odsCode)
    }

    override fun connectionToExternalServiceFailed() {
        throw NotImplementedError("Not implemented for this GP system")
    }

    private fun emisValidCredentialsWithNHSNumbers(numbers: List<String>) {
        val patient = Patient(
                name = PatientName(title = "Miss",
                        firstName = "Alexia",
                        surname = "Scott"),
                odsCode = EmisMockDefaults.DEFAULT_ODS_CODE_EMIS,
                connectionToken = "fe81f191-b016-466e-aeb2-64f08f2330a4",
                sessionId = "xkWiivK1WBAkxIN9CDrGyy",
                endUserSessionId = "9RFDWiqTO8zBWrp2p8s4K7",
                userPatientLinkToken = "KxLiDl5nRS60DzIlrKoFSl",
                nhsNumbers = numbers,
                age = PatientAge(dateOfBirth = "1985-05-29"))

        val nhsNumbers = numbers.map { number ->
            PatientIdentifier(number,
                    identifierType = IdentifierType.NhsNumber)
        }.toTypedArray()

        mockingClient.forEmis.mock {
            authentication.endUserSessionRequest()
                    .respondWithSuccess(patient.endUserSessionId)
        }
        mockingClient.forEmis.mock {
            authentication.sessionRequest(patient)
                    .respondWithSuccess(patient, AssociationType.Self)
        }
        mockingClient.forEmis.mock {
            myRecord.demographicsRequest(patient)
                    .respondWithSuccess(patient, nhsNumbers)
        }

        PatientVerificationSerenityHelpers.ConnectionToken.set(patient.connectionToken)
        PatientVerificationSerenityHelpers.NationalPracticeCode.set(patient.odsCode)
        PatientVerificationSerenityHelpers.NhsNumbers.set(nhsNumbers)

    }

    override fun setSessionExtendMockResponse(patient: Patient, expectedResponse: String) {

        mockingClient.forEmis.mock {
            practiceSettingsRequest(patient).respondWith(
                    HttpStatus.SC_OK, resolve = {}, milliSecondDelay = 0)
        }

        when (expectedResponse) {
            "Success" -> {
                mockingClient.forEmis.mock {
                    authentication.demographicsRequest(patient).respondWithSuccess(patient,
                            patientIdentifiers = arrayOf(
                                    PatientIdentifier(
                                            identifierType = IdentifierType.NhsNumber,
                                            identifierValue = patient.nhsNumbers[0]
                                    )
                            )
                    )
                }
            }
            "bad gateway" -> {
                mockingClient.forEmis.mock {
                    authentication.demographicsRequest(patient)
                            .respondWithExceptionWhenNotEnabled()
                            .whenScenarioStateIs("sessionStarted")
                }
            }
            "gateway timeout" -> {
                mockingClient.forEmis.mock {
                    authentication.demographicsRequest(patient).respondWith(
                            HttpStatus.SC_OK, resolve = {}, milliSecondDelay = 0)
                            .delayedBy(seconds = Duration.ofSeconds(REQUEST_DELAY))
                }
            }
            "unauthorized" -> {
                mockingClient.forEmis.mock {
                    authentication.demographicsRequest(patient)
                            .respondWith(HttpStatus.SC_UNAUTHORIZED,
                                    resolve = {}, milliSecondDelay = 0)
                }
            }
        }
    }

    override fun oldOdsCodeAndConnectionTokenForPatientThatHasSinceMovedToADifferentPractice() {
        throw NotImplementedError("Not implemented for this GP system")
    }

    override fun gpSystemNotAvailable() {
        val patient = EmisMockDefaults.patientEmis
        mockingClient.forEmis.mock {
            authentication.endUserSessionRequest()
                    .respondWithServiceUnavailable()
        }

        PatientVerificationSerenityHelpers.ConnectionToken.set(patient.connectionToken)
        PatientVerificationSerenityHelpers.NationalPracticeCode.set(patient.odsCode)
    }
}
