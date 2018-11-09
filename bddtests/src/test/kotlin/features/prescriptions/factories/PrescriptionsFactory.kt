package features.prescriptions.factories

import features.sharedSteps.SerenityHelpers
import features.sharedSteps.SupplierSpecificFactory
import mocking.MockingClient
import mocking.data.prescriptions.IPrescriptionLoader
import mocking.gpServiceBuilderInterfaces.courses.ICoursesLoader
import models.Patient

abstract class PrescriptionsFactory(gpSupplier:String) {

    abstract val getCoursesLoader: ICoursesLoader<*>
    abstract val getPrescriptionsLoader: IPrescriptionLoader<*>

    fun setupWireMockAndCreateData(numOfCourses: Int,
                                   numOfRepeats: Int,
                                   numCanBeRequested: Int,
                                   showDosage: Boolean,
                                   includeQuantity: Boolean) {

        getCoursesLoader.loadData(numOfCourses,
                numOfRepeats,
                numCanBeRequested,
                showDosage,
                includeQuantity)

        setupWireMockAndCreateDataGpSpecific()
    }

    abstract fun setupWireMockAndDataSetup(scenarioTitle: String,
                                           initialScenarioState: String,
                                           statusSubmitted: String,
                                           initialHistoricPrescriptionsCount: Int,
                                           amount: Int): String
    abstract fun disableAtGPLevel()
    abstract fun setupWireMockAndCreateDataGpSpecific()

    val mockingClient = MockingClient.instance
    val patient = SerenityHelpers.getPatientOrNull() ?: Patient.getDefault(gpSupplier)

    companion object : SupplierSpecificFactory<PrescriptionsFactory>() {

        override val map: HashMap<String, (() -> PrescriptionsFactory)>
                by lazy {
                    hashMapOf(
                            "EMIS" to { PrescriptionsFactoryEmis() },
                            "TPP" to { PrescriptionsFactoryTpp() },
                            "VISION" to { PrescriptionsFactoryVision() })
                }

    }
}
