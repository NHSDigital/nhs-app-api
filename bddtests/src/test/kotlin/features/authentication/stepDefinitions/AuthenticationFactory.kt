package features.authentication.stepDefinitions

import features.sharedSteps.SerenityHelpers
import features.sharedSteps.SupplierSpecificFactory
import mocking.emis.models.AssociationType
import models.Patient

abstract class AuthenticationFactory(protected val gpSystem: String)  {

    protected val patient:Patient = Patient.getDefault(gpSystem)
    protected val mockingClient = SerenityHelpers.getMockingClient()
    protected val associationType = AssociationType.Self

    init {
        SerenityHelpers.setPatient(patient)
    }

    abstract fun validOAuthDetailsAndGpSystemUnavailable()
    abstract fun validOAuthDetailsCidConnectionTokenFailsToAuthenticate()
    abstract fun validOAuthDetailsAndGpSystemSlowToRespond()

    companion object : SupplierSpecificFactory<AuthenticationFactory>() {

        override val map: HashMap<String, (()-> AuthenticationFactory)> by lazy {
            hashMapOf(
                    "EMIS" to { AuthenticationFactoryEmis() },
                    "TPP" to { AuthenticationFactoryTpp() },
                    "VISION" to { AuthenticationFactoryVision() })}

    }
}
