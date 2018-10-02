package features.sharedStepDefinitions


import models.Patient

const val GLOBAL_PROVIDER_TYPE = "GLOBAL_PROVIDER_TYPE"
open class BaseStepDefinition {

    companion object {
        enum class ProviderTypes {
            EMIS, TPP, VISION
        }
    }

    val EMIS_PATIENT = Patient.getDefault(ProviderTypes.EMIS.toString())
    val TPP_PATIENT = Patient.getDefault(ProviderTypes.TPP.toString())
    val VISION_PATIENT = Patient.getDefault(ProviderTypes.VISION.toString())

    var currentProvider: ProviderTypes? = null

    lateinit var currentPatient: Patient
}
