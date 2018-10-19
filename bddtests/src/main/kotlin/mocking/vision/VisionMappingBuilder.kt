package mocking.vision

import mocking.MappingBuilder
import mocking.vision.Demographics.VisionDemographicsBuilder
import mocking.vision.models.ServiceDefinition
import mocking.vision.models.VisionUserSession
import mocking.defaults.MockDefaults
import mocking.vision.models.OrderNewPrescriptionRequest

open class VisionMappingBuilder(method: String = "POST") : MappingBuilder(method, "/vision/") {

    var appointments = VisionMappingBuilderAppointments()

    fun getConfigurationRequest(visionUserSession: VisionUserSession) =
            VisionGetConfigurationBuilder(visionUserSession, MockDefaults.visionGetConfiguration)

    fun getPrescriptionHistoryRequest(visionUserSession: VisionUserSession) =
            VisionGetHistoricPrescriptionsBuilder(visionUserSession, MockDefaults.visionGetHistory)

    fun getEligibleRepeatsRequest(visionUserSession: VisionUserSession) =
            VisionEligibleRepeatsBuilder(visionUserSession, MockDefaults.visionGetEligibleRepeats)

    fun orderNewPrescriptionRequest(
            visionUserSession: VisionUserSession,
            orderNewPrescriptionRequest: OrderNewPrescriptionRequest) =
            VisionOrderNewPrescriptionBuilder(
                    visionUserSession,
                    MockDefaults.visionOrderNewPrescription,
                    orderNewPrescriptionRequest
            )

    fun demographicsRequest(visionUserSession: VisionUserSession) =
            VisionDemographicsBuilder(visionUserSession, MockDefaults.visionDemographicsConfiguration)

    fun getPatientDataRequest(visionUserSession: VisionUserSession, serviceDefinition: ServiceDefinition, view:
            String, responseFormat: String)
            = VisionGetPatientDataBuilder(visionUserSession, serviceDefinition, view, responseFormat)
}
