package mocking.vision

import constants.ErrorResponseCodeVision
import mocking.MappingBuilder
import mocking.models.Mapping
import mocking.vision.demographics.VisionDemographicsBuilder
import mocking.vision.helpers.VisionConstantsHelper
import mocking.vision.models.VisionUserSession
import mocking.vision.models.OrderNewPrescriptionRequest
import mocking.vision.models.ServiceDefinition
import models.Patient
import org.apache.http.HttpStatus

open class VisionMappingBuilder(method: String = "POST") : MappingBuilder(method, "/vision/") {

    var appointments = VisionMappingBuilderAppointments()

    fun getConfigurationRequest(visionUserSession: VisionUserSession): VisionGetConfigurationBuilder {
        return VisionGetConfigurationBuilder(visionUserSession,
                VisionMockDefaults.visionGetConfiguration)
    }

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

    fun getPatientDataRequest(visionUserSession: VisionUserSession, serviceDefinition: ServiceDefinition,
                              view: String, responseFormat: String): VisionGetPatientDataBuilder {
        return VisionGetPatientDataBuilder(visionUserSession,
                serviceDefinition, view, responseFormat)
    }


    fun getRegisterRequest(visionUserSession: VisionUserSession, patient: Patient):VisionRegisterBuilder {
        return VisionRegisterBuilder(visionUserSession, VisionMockDefaults.visionGetRegister, patient)
    }

    fun demographicsRequest(visionUserSession: VisionUserSession): VisionDemographicsBuilder {
        return VisionDemographicsBuilder(visionUserSession,
                VisionMockDefaults.visionDemographicsConfiguration)
    }

    fun respondWithCorruptedContent(serviceDefinition: ServiceDefinition, content: String = ""): Mapping {
        return respondWith(HttpStatus.SC_OK) {
            val corruptedResponse = VisionConstantsHelper
                    .getBaseVisionResponse(content, serviceDefinition)
                    .replace(">", "|")
                    .replace("}", "|")

            andBody(corruptedResponse, contentType = "text/xml")
        }
    }

    fun respondVisionErrorWhenServiceNotEnabled(serviceDefinition: ServiceDefinition):Mapping {
        return respondWith(HttpStatus.SC_OK) {
            andXmlBody(VisionConstantsHelper.getBaseVisionFailedResponse(
                    serviceDefinition,
                    ErrorResponseCodeVision.ACCESS_DENIED)).build()
        }
    }

    fun respondWithUnknownVisionError(serviceDefinition: ServiceDefinition):Mapping {
        return respondWith(HttpStatus.SC_OK) {
            andXmlBody(VisionConstantsHelper.getBaseVisionFailedResponse(
                    serviceDefinition,
                    ErrorResponseCodeVision.NON_VISION_ERROR_CODE)).build()
        }
    }
}
