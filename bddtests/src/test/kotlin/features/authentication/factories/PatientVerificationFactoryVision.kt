package features.authentication.factories

import mocking.emis.demographics.PatientIdentifier
import mocking.emis.models.IdentifierType
import mocking.defaults.VisionMockDefaults
import mocking.vision.models.Account
import mocking.vision.models.Configuration
import mocking.vision.models.PatientNumber
import mocking.vision.models.VisionUserSession
import models.Patient
import net.serenitybdd.core.Serenity

class PatientVerificationFactoryVision: PatientVerificationFactory("VISION"){

    override fun setSessionExtendMockResponse(patient: Patient, expectedResponse: String) {
        // Currently no session mocking required for vision
    }

    override fun validPatientWithNoNhsNumber() {
        val patient =  Patient.getDefault(gpSystem)

        mockingClient
                .forVision {
                    authentication.getConfigurationRequest(
                            visionUserSession = VisionUserSession.fromPatient(patient))
                            .respondWithSuccess(configuration = Configuration(account = Account(patient.patientId,
                                    patientNumber = null, name = patient.formattedFullName())
                            ))
                }
        Serenity.setSessionVariable("ConnectionToken").to(patient.connectionToken)
        Serenity.setSessionVariable("NationalPracticeCode").to(patient.odsCode)
        Serenity.setSessionVariable("NhsNumber").to("")
    }

    override fun validPatientWithMultipleNumbers() {
        val patient = Patient.getDefault(gpSystem)
        visionValidCredentialsWithNHSNumbers(arrayOf(patient.nhsNumbers.first(), "1231231234"))
    }

    override fun validPatientWithOneNhsNumber() {
        val patient = Patient.getDefault(gpSystem)
        visionValidCredentialsWithNHSNumbers(arrayOf(patient.nhsNumbers.first()))
    }


    override val odsCode: String = VisionMockDefaults.DEFAULT_ODS_CODE_VISION

    override fun im1ConnectionTokenDoesNotExist() {
        val patient = VisionMockDefaults.patientVision
        val nonExistingConnectionToken = "{\"rosuAccountid\":\"999999999\",\"apiKey\":\"nonexistingapikey\"}"

        mockingClient
                .forVision {
                    authentication.getConfigurationRequest(
                            visionUserSession = VisionUserSession(
                                    "999999999",
                                    "nonexistingapikey",
                                    patient.odsCode, patientId = patient.patientId))
                            .respondWitInvalidUserCredentials()
                }
        Serenity.setSessionVariable("ConnectionToken").to(nonExistingConnectionToken)
        Serenity.setSessionVariable("NationalPracticeCode").to(patient.odsCode)
        Serenity.setSessionVariable("NhsNumber").to(patient.nhsNumbers[0])
    }


    private fun visionValidCredentialsWithNHSNumbers(nhsNumbers: Array<String>){
        val patient = VisionMockDefaults.patientVision
        mockingClient
                .forVision {
                    authentication.getConfigurationRequest(
                            visionUserSession = VisionUserSession(
                                    patient.rosuAccountId,
                                    patient.apiKey,
                                    patient.odsCode,
                                    patient.patientId))
                            .respondWithSuccess(configuration = Configuration(
                                    account = Account(patient.patientId,
                                            patientNumber = nhsNumbers.map { number -> PatientNumber(number = number) },
                                            name = patient.formattedFullName()
                                    )))
                }
        Serenity.setSessionVariable("ConnectionToken").to(patient.connectionToken)
        Serenity.setSessionVariable("NationalPracticeCode").to(patient.odsCode)
        Serenity.setSessionVariable("NhsNumbers").to(nhsNumbers.map { number -> PatientIdentifier(number,
                IdentifierType.NhsNumber) }.toTypedArray())
    }
}
