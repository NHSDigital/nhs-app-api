package mocking.vision

import mocking.MappingBuilder
import mocking.vision.Demographics.VisionDemographicsBuilder
import mocking.vision.models.ServiceDefinition
import mocking.vision.models.VisionUserSession
import models.Patient

open class VisionMappingBuilder(private val method: String) : MappingBuilder(method, "/vision/") {

    fun getConfigurationRequest(visionUserSession: VisionUserSession, serviceDefinition: ServiceDefinition)
            = VisionGetConfigurationBuilder(visionUserSession, serviceDefinition)

    fun getPrescriptionHistoryRequest(visionUserSession: VisionUserSession, serviceDefinition: ServiceDefinition)
            = VisionGetHistoricPrescriptionsBuilder(visionUserSession, serviceDefinition)

    fun getEligibleRepeatsRequest(visionUserSession: VisionUserSession, serviceDefinition: ServiceDefinition)
            = VisionEligibleRepeatsBuilder(visionUserSession, serviceDefinition)

    fun demographicsRequest(visionUserSession: VisionUserSession, serviceDefinition: ServiceDefinition)
            = VisionDemographicsBuilder(visionUserSession, serviceDefinition)


}