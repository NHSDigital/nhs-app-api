package features.sharedStepDefinitions

import models.Patient

open class BaseStepDefinition {

    companion object {
        enum class ProviderTypes {
            EMIS, TPP, VISION, MICROTEST
        }
    }

    var currentProvider: ProviderTypes? = null

    lateinit var currentPatient: Patient
}
