package mocking.vision

import mocking.MappingBuilder
import mocking.defaults.VisionMockDefaults
import mocking.vision.models.OrderNewPrescriptionRequest
import mocking.vision.models.VisionUserSession

open class VisionMappingBuilderPrescriptions(method: String = "POST") : MappingBuilder(method, "/vision/") {

    fun getPrescriptionHistoryRequest(visionUserSession: VisionUserSession): VisionGetHistoricPrescriptionsBuilder {
        return VisionGetHistoricPrescriptionsBuilder(visionUserSession, VisionMockDefaults
                .visionGetPrescriptionHistory)
    }

    fun getEligibleRepeatsRequest(visionUserSession: VisionUserSession): VisionEligibleRepeatsBuilder {
        return VisionEligibleRepeatsBuilder(visionUserSession,
                VisionMockDefaults.visionGetEligibleRepeats)
    }

    fun orderNewPrescriptionRequest(visionUserSession: VisionUserSession, orderNewPrescriptionRequest:
    OrderNewPrescriptionRequest): VisionOrderNewPrescriptionBuilder {
        return VisionOrderNewPrescriptionBuilder(
                visionUserSession,
                VisionMockDefaults.visionOrderNewPrescription,
                orderNewPrescriptionRequest)
    }
}
