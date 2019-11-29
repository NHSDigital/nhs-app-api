package features.authentication.factories

import constants.Supplier
import features.authentication.stepDefinitions.PatientVerificationSerenityHelpers
import mocking.emis.demographics.PatientIdentifier
import mocking.emis.models.IdentifierType
import mocking.defaults.MicrotestMockDefaults
import models.Patient
import utils.SerenityHelpers
import utils.set

class PatientVerificationFactoryMicrotest: PatientVerificationFactory(Supplier.MICROTEST){
    override fun setSessionExtendMockResponse(patient: Patient, expectedResponse: String) {
        // Currently no session mocking required for vision
    }

    override val odsCode: String = MicrotestMockDefaults.DEFAULT_ODS_CODE_MICROTEST


    override fun validPatientWithNoNhsNumber() {
        throw NotImplementedError("It is not possible for there to be a user with no NHS number for Microtest")
    }

    override fun validPatientWithMultipleNumbers() {
        throw NotImplementedError("It is not possible for there to be a user with multiple NHS number for Microtest")
    }

    override fun validPatientWithOneNhsNumber() {
        val patient = Patient.getDefault(gpSystem)
        SerenityHelpers.setPatient(patient)
        PatientVerificationSerenityHelpers.ConnectionToken.set(patient.connectionToken)
        PatientVerificationSerenityHelpers.NationalPracticeCode.set(patient.odsCode)
        PatientVerificationSerenityHelpers.NhsNumbers.set(arrayOf(patient.nhsNumbers.first())
                .map { number -> PatientIdentifier(number, IdentifierType.NhsNumber) }
                .toTypedArray())

        mockingClient.forMicrotest {
            demographics.demographicsRequest(patient)
                    .respondWithSuccess()
        }
    }

    override fun im1ConnectionTokenDoesNotExist() {
        throw NotImplementedError("For Microtest this is covered by the 'moved to a different practice' scenario")
    }

    override fun connectionToExternalServiceFailed() {
        throw NotImplementedError("Not implemented for this GP system")
    }

    override fun oldOdsCodeAndConnectionTokenForPatientThatHasSinceMovedToADifferentPractice() {
        val patient = Patient.getDefault(gpSystem)
        SerenityHelpers.setPatient(patient)

        mockingClient.forMicrotest {
            demographics.demographicsRequest(patient)
                    .respondWithInternalServerError()
        }

        PatientVerificationSerenityHelpers.ConnectionToken.set(patient.connectionToken)
        PatientVerificationSerenityHelpers.NationalPracticeCode.set(patient.odsCode)
    }

    override fun gpSystemNotAvailable() {
        val patient = MicrotestMockDefaults.patient
        SerenityHelpers.setPatient(patient)

        mockingClient.forMicrotest {
            demographics.demographicsRequest(patient).respondWithServiceUnavailable()
        }

        PatientVerificationSerenityHelpers.ConnectionToken.set(patient.connectionToken)
        PatientVerificationSerenityHelpers.NationalPracticeCode.set(patient.odsCode)
    }
}
