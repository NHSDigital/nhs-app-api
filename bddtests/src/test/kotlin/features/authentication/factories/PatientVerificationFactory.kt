package features.authentication.factories

import constants.Supplier
import mocking.SupplierSpecificFactory
import models.Patient
import utils.SerenityHelpers

abstract class PatientVerificationFactory(protected val gpSystem: Supplier)  {

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

        override val map: HashMap<Supplier, (()-> PatientVerificationFactory)> by lazy {
            hashMapOf(
                    Supplier.EMIS to { PatientVerificationFactoryEmis() },
                    Supplier.TPP to { PatientVerificationFactoryTpp() },
                    Supplier.VISION to { PatientVerificationFactoryVision() },
                    Supplier.MICROTEST to { PatientVerificationFactoryMicrotest() } )}
    }
}
