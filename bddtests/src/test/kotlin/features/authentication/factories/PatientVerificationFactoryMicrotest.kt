package features.authentication.factories

import mocking.emis.demographics.PatientIdentifier
import mocking.emis.models.IdentifierType
import mocking.defaults.MicrotestMockDefaults
import models.Patient
import net.serenitybdd.core.Serenity

class PatientVerificationFactoryMicrotest: PatientVerificationFactory("MICROTEST"){

    override fun setSessionExtendMockResponse(patient: Patient, expectedResponse: String) {
        // Currently no session mocking required for vision
    }

    override val odsCode: String = MicrotestMockDefaults.DEFAULT_ODS_CODE_MICROTEST


    override fun validPatientWithNoNhsNumber() {
        val patient =  Patient.getDefault(gpSystem)

        Serenity.setSessionVariable("ConnectionToken").to(patient.connectionToken)
        Serenity.setSessionVariable("NationalPracticeCode").to(patient.odsCode)
        Serenity.setSessionVariable("NhsNumber").to("")
    }

    override fun validPatientWithMultipleNumbers() {
        val patient = Patient.getDefault(gpSystem)
        Serenity.setSessionVariable("ConnectionToken").to(patient.connectionToken)
        Serenity.setSessionVariable("NationalPracticeCode").to(patient.odsCode)
        Serenity.setSessionVariable("NhsNumbers").to(arrayOf(patient.nhsNumbers.first(), "1231231234")
                .map { number -> PatientIdentifier(number, IdentifierType.NhsNumber) }
                .toTypedArray())
    }

    override fun validPatientWithOneNhsNumber() {
        val patient = Patient.getDefault(gpSystem)
        Serenity.setSessionVariable("ConnectionToken").to(patient.connectionToken)
        Serenity.setSessionVariable("NationalPracticeCode").to(patient.odsCode)
        Serenity.setSessionVariable("NhsNumbers").to(arrayOf(patient.nhsNumbers.first())
                .map { number -> PatientIdentifier(number, IdentifierType.NhsNumber) }
                .toTypedArray())
    }

    override fun im1ConnectionTokenDoesNotExist() {
        val patient = MicrotestMockDefaults.patient
        val nonExistingConnectionToken = "{\"rosuAccountid\":\"999999999\",\"apiKey\":\"nonexistingapikey\"}"

        Serenity.setSessionVariable("ConnectionToken").to(nonExistingConnectionToken)
        Serenity.setSessionVariable("NationalPracticeCode").to(patient.odsCode)
        Serenity.setSessionVariable("NhsNumber").to(patient.nhsNumbers[0])
    }
}
