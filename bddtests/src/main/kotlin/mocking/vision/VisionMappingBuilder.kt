package mocking.vision

import mocking.MappingBuilder
import mocking.vision.Demographics.VisionDemographicsBuilder
import mocking.vision.allergies.VisionAllergiesBuilder
import mocking.vision.models.ServiceDefinition
import mocking.vision.models.VisionUserSession
import mocking.defaults.MockDefaults
import mocking.vision.models.OrderNewPrescriptionRequest

open class VisionMappingBuilder(private val method: String) : MappingBuilder(method, "/vision/") {

    fun getConfigurationRequest(visionUserSession: VisionUserSession)
            = VisionGetConfigurationBuilder(visionUserSession, MockDefaults.visionGetConfiguration)

    fun getPrescriptionHistoryRequest(visionUserSession: VisionUserSession)
            = VisionGetHistoricPrescriptionsBuilder(visionUserSession, MockDefaults.visionGetHistory)

    fun getEligibleRepeatsRequest(visionUserSession: VisionUserSession)
            = VisionEligibleRepeatsBuilder(visionUserSession, MockDefaults.visionGetEligibleRepeats)

    fun orderNewPrescriptionRequest(
            visionUserSession: VisionUserSession,
            orderNewPrescriptionRequest: OrderNewPrescriptionRequest)
            = VisionOrderNewPrescriptionBuilder(
            visionUserSession,
            MockDefaults.visionOrderNewPrescription,
            orderNewPrescriptionRequest)

    fun allergiesRequest(visionUserSession: VisionUserSession, serviceDefinition: ServiceDefinition)
            = VisionAllergiesBuilder(visionUserSession, serviceDefinition)

    fun demographicsRequest(visionUserSession: VisionUserSession)
            = VisionDemographicsBuilder(visionUserSession, MockDefaults.visionDemographicsConfiguration)
}

