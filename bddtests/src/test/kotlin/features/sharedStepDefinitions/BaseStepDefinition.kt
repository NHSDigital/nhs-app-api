package features.sharedStepDefinitions


import models.Patient

const val GLOBAL_PROVIDER_TYPE = "GLOBAL_PROVIDER_TYPE"

open class BaseStepDefinition {

    companion object {
        enum class ProviderTypes {
            EMIS, TPP, VISION
        }
    }

    var currentProvider: ProviderTypes? = null

    lateinit var currentPatient: Patient
}
