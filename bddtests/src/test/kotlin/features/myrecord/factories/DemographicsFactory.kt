package features.myrecord.factories

import constants.Supplier
import mocking.SupplierSpecificFactory
import mocking.MockingClient
import models.Patient

abstract class DemographicsFactory {

    val mockingClient = MockingClient.instance

    abstract fun disabled(patient: Patient)
    abstract fun enabled(patient: Patient)
    abstract fun enabledViaProxy(callingPatient: Patient, actingOnBehalfOf: Patient)
    abstract fun enabledButTimesOut(patient: Patient)
    abstract fun throwInternalError(patient: Patient)
    abstract fun throwForbiddenError(patient: Patient)
    abstract fun throwBadGateway(patient: Patient)

    companion object : SupplierSpecificFactory<DemographicsFactory>() {

        override val map: HashMap<Supplier, (() -> DemographicsFactory)>
                by lazy {
                    hashMapOf(
                            Supplier.EMIS to { DemographicsFactoryEmis() },
                            Supplier.TPP to { DemographicsFactoryTpp() },
                            Supplier.VISION to { DemographicsFactoryVision() },
                            Supplier.MICROTEST to { DemographicsFactoryMicrotest() })
                }
    }
}
