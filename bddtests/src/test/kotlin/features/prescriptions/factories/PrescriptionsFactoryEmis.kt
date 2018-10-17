package features.prescriptions.factories

import mocking.data.prescriptions.EmisPrescriptionLoader
import mocking.data.prescriptions.IPrescriptionLoader

class PrescriptionsFactoryEmis: PrescriptionsFactory("EMIS"){

    override val getPrescriptionsLoader: IPrescriptionLoader<*>
        get() = EmisPrescriptionLoader


    override fun disableAtGPLevel() {
        mockingClient
                .forEmis {
                    prescriptions.prescriptionsRequest(patient)
                            .respondWithPrescriptionsNotEnabled()
                }
    }
}