package features.authentication.stepDefinitions

import constants.Supplier
import utils.SerenityHelpers
import mocking.SupplierSpecificFactory
import mocking.emis.models.AssociationType
import models.Patient

abstract class AuthenticationFactory(protected val gpSystem: Supplier) {

    protected val mockingClient = SerenityHelpers.getMockingClient()
    protected val associationType = AssociationType.Self
    protected var patient: Patient

    init {
        SerenityHelpers.setPatientIfNull(Patient.getDefault(gpSystem))
        patient = SerenityHelpers.getPatient()
    }

    abstract fun validOAuthDetailsAndGpSystemUnavailable(patient: Patient)
    abstract fun validOAuthDetailsAndGpSystemBadGateway()
    abstract fun validOAuthDetailsCidConnectionTokenFailsToAuthenticate()
    abstract fun validOAuthDetailsAndGpSystemSlowToRespond(delayBySeconds: Long)
    abstract fun validOAuthDetailsAndGpSystemReturnsError()
    abstract fun patientDoesNotExist(patient: Patient)
    abstract fun patientWithIncorrectLinkageKey(patient: Patient)
    abstract fun patientWithIncorrectSurname(patient: Patient)
    abstract fun patientWithIncorrectDOB(patient: Patient)
    abstract fun patientWithSurnameInWrongFormat(patient: Patient)
    abstract fun patientWithAccountIDInWrongFormat(patient: Patient)
    abstract fun patientWithLinkageKeyInWrongFormat(patient: Patient)
    abstract fun patientWithDOBInWrongFormat(patient: Patient)
    abstract fun patientWithIncompleteResponse(patient: Patient)


    companion object : SupplierSpecificFactory<AuthenticationFactory>() {

        override val map: HashMap<Supplier, (() -> AuthenticationFactory)> by lazy {
            hashMapOf(
                    Supplier.EMIS to { AuthenticationFactoryEmis() },
                    Supplier.TPP to { AuthenticationFactoryTpp() },
                    Supplier.VISION to { AuthenticationFactoryVision() },
                    Supplier.MICROTEST to { AuthenticationFactoryMicrotest() }
            )
        }
    }
}
