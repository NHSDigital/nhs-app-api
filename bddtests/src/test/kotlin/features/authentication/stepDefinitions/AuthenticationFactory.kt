package features.authentication.stepDefinitions

import utils.SerenityHelpers
import features.sharedSteps.SupplierSpecificFactory
import mocking.emis.models.AssociationType
import models.Patient

abstract class AuthenticationFactory(protected val gpSystem: String) {

    protected val patient: Patient = Patient.getDefault(gpSystem)
    protected val mockingClient = SerenityHelpers.getMockingClient()
    protected val associationType = AssociationType.Self

    init {
        SerenityHelpers.setPatient(patient)
    }

    abstract fun validOAuthDetailsAndGpSystemUnavailable()
    abstract fun validOAuthDetailsCidConnectionTokenFailsToAuthenticate()
    abstract fun validOAuthDetailsAndGpSystemSlowToRespond(delayBySeconds: Long)
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

        override val map: HashMap<String, (() -> AuthenticationFactory)> by lazy {
            hashMapOf(
                    "EMIS" to { AuthenticationFactoryEmis() },
                    "TPP" to { AuthenticationFactoryTpp() },
                    "VISION" to { AuthenticationFactoryVision() },
                    "MICROTEST" to { AuthenticationFactoryMicrotest() }
            )
        }
    }
}
