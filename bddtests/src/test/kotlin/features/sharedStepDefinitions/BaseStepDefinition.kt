package features.sharedStepDefinitions


import models.Patient

open class BaseStepDefinition {

    companion object {
        enum class ProviderTypes {
            EMIS, TPP
        }

        val GLOBAL_PROVIDER_TYPE = "GLOBAL_PROVIDER_TYPE"
    }

    val EMIS_PATIENT = Patient.getDefault(ProviderTypes.EMIS.toString())
    val TPP_PATIENT = Patient.getDefault(ProviderTypes.TPP.toString())

    var currentProvider: ProviderTypes? = null

    lateinit var currentPatient: Patient
}