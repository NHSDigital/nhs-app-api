package features.authentication.factories

import features.sharedSteps.SupplierSpecificFactory
import models.Patient
import utils.SerenityHelpers

abstract class PatientVerificationFactory(protected val gpSystem: String)  {

    protected val mockingClient = SerenityHelpers.getMockingClient()
    abstract val odsCode: String

    abstract fun im1ConnectionTokenDoesNotExist()
    abstract fun validPatientWithOneNhsNumber()
    abstract fun validPatientWithMultipleNumbers()
    abstract fun validPatientWithNoNhsNumber()
    abstract fun setSessionExtendMockResponse(patient: Patient, expectedResponse: String)


    companion object : SupplierSpecificFactory<PatientVerificationFactory>() {

        override val map: HashMap<String, (()-> PatientVerificationFactory)> by lazy {
            hashMapOf(
                    "EMIS" to { PatientVerificationFactoryEmis() },
                    "TPP" to { PatientVerificationFactoryTpp() },
                    "VISION" to { PatientVerificationFactoryVision() },
                    "MICROTEST" to { PatientVerificationFactoryMicrotest() } )}
    }
}
