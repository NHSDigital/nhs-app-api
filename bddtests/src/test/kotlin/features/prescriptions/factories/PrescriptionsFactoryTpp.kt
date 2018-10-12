package features.prescriptions.factories

import mocking.data.prescriptions.IPrescriptionLoader
import mocking.data.prescriptions.TppPrescriptionLoader

class PrescriptionsFactoryTpp: PrescriptionsFactory("TPP"){

    override val getPrescriptionsLoader: IPrescriptionLoader<*>
        get() = TppPrescriptionLoader

    override fun disableAtGPLevel() {
        mockingClient
                .forTpp {
                    listRepeatMedication(patient)
                            .respondWith(403, 0, resolve = {})
                }
    }
}