package features.authentication.factories

import mocking.SupplierSpecificFactory
import models.Patient
import utils.SerenityHelpers

abstract class PatientVerificationFactory(protected val gpSystem: String)  {

    protected val mockingClient = SerenityHelpers.getMockingClient()
    abstract val odsCode: String

    abstract fun im1ConnectionTokenDoesNotExist()
    abstract fun connectionToExternalServiceFailed()
    abstract fun validPatientWithOneNhsNumber()
    abstract fun validPatientWithMultipleNumbers()
    abstract fun validPatientWithNoNhsNumber()
    abstract fun setSessionExtendMockResponse(patient: Patient, expectedResponse: String = "Success")
    abstract fun oldOdsCodeAndConnectionTokenForPatientThatHasSinceMovedToADifferentPractice()
    abstract fun gpSystemNotAvailable()


    companion object : SupplierSpecificFactory<PatientVerificationFactory>() {

        override val map: HashMap<String, (()-> PatientVerificationFactory)> by lazy {
            hashMapOf(
                    "EMIS" to { PatientVerificationFactoryEmis() },
                    "TPP" to { PatientVerificationFactoryTpp() },
                    "VISION" to { PatientVerificationFactoryVision() },
                    "MICROTEST" to { PatientVerificationFactoryMicrotest() } )}
    }
}
