package mocking.vision

import mocking.MappingBuilder
import mocking.defaults.VisionMockDefaults
import mocking.vision.demographics.VisionGetDemographicsBuilder
import mocking.vision.models.ServiceDefinition
import mocking.vision.models.VisionUserSession

open class VisionMappingBuilderMyRecord(method: String = "POST") : MappingBuilder(method, "/vision/") {

    fun getPatientDataRequest(visionUserSession: VisionUserSession, serviceDefinition: ServiceDefinition,
                              view: String, responseFormat: String): VisionGetPatientDataBuilder {
        return VisionGetPatientDataBuilder(visionUserSession,
                serviceDefinition, view, responseFormat)
    }

    fun demographicsRequest(visionUserSession: VisionUserSession): VisionGetDemographicsBuilder {
        return VisionGetDemographicsBuilder(visionUserSession,
                VisionMockDefaults.visionDemographicsConfiguration)
    }
}
