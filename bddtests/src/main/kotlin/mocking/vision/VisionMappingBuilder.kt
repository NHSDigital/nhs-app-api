package mocking.vision

import mocking.MappingBuilder
import mocking.vision.Demographics.VisionDemographicsBuilder
import mocking.vision.models.VisionUserSession
import mocking.vision.models.OrderNewPrescriptionRequest
import mocking.vision.models.ServiceDefinition
import models.Patient

open class VisionMappingBuilder(method: String = "POST") : MappingBuilder(method, "/vision/") {

    var appointments = VisionMappingBuilderAppointments()

    fun getConfigurationRequest(visionUserSession: VisionUserSession)
            = VisionGetConfigurationBuilder(visionUserSession, VisionMockDefaults.visionGetConfiguration)

    fun getPrescriptionHistoryRequest(visionUserSession: VisionUserSession)
            = VisionGetHistoricPrescriptionsBuilder(visionUserSession, VisionMockDefaults
            .visionGetPrescriptionHistory)

    fun getEligibleRepeatsRequest(visionUserSession: VisionUserSession)
            = VisionEligibleRepeatsBuilder(visionUserSession, VisionMockDefaults.visionGetEligibleRepeats)

    fun orderNewPrescriptionRequest(
            visionUserSession: VisionUserSession,
            orderNewPrescriptionRequest: OrderNewPrescriptionRequest)
            = VisionOrderNewPrescriptionBuilder(
            visionUserSession,
            VisionMockDefaults.visionOrderNewPrescription,
            orderNewPrescriptionRequest)

    fun getPatientDataRequest(visionUserSession: VisionUserSession, serviceDefinition: ServiceDefinition, view:
    String, responseFormat: String)
            = VisionGetPatientDataBuilder(visionUserSession, serviceDefinition, view, responseFormat)


    fun getRegisterRequest
            (visionUserSession: VisionUserSession, patient: Patient) =
            VisionRegisterBuilder(visionUserSession, VisionMockDefaults.visionGetRegister, patient)

    fun demographicsRequest(visionUserSession: VisionUserSession)
            = VisionDemographicsBuilder(visionUserSession, VisionMockDefaults.visionDemographicsConfiguration)
}
