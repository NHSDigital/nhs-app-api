package features.prescriptions.factories

import mocking.data.prescriptions.IPrescriptionLoader
import mocking.data.prescriptions.VisionPrescriptionLoader

class PrescriptionsFactoryVision: PrescriptionsFactory("VISION"){

    override val getPrescriptionsLoader: IPrescriptionLoader<*>
        get() = VisionPrescriptionLoader

    override fun disableAtGPLevel() {   }
}