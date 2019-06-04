package mocking.vision

import mocking.MappingBuilder
import mocking.defaults.VisionMockDefaults
import mocking.vision.linkage.VisionLinkageGETBuilder
import mocking.vision.linkage.VisionLinkagePOSTBuilder
import mocking.vision.models.VisionUserSession
import mocking.vision.models.linkage.LinkageKeyPostRequest
import models.Patient

open class VisionMappingBuilderAuthentication(method: String = "POST") : MappingBuilder(method, "/vision/") {

    fun getConfigurationRequest(visionUserSession: VisionUserSession): VisionGetConfigurationBuilder {
        return VisionGetConfigurationBuilder(visionUserSession,
                VisionMockDefaults.visionGetConfiguration)
    }

    fun getRegisterRequest(visionUserSession: VisionUserSession, patient: Patient):VisionRegisterBuilder {
        return VisionRegisterBuilder(visionUserSession, VisionMockDefaults.visionGetRegister, patient)
    }

    fun linkageKeyGetRequest(orgId: String, nhsNumber: String)= VisionLinkageGETBuilder (orgId, nhsNumber)

    fun linkageKeyPostRequest(orgId: String,  linkageKeyPostRequest: LinkageKeyPostRequest)=
            VisionLinkagePOSTBuilder (orgId, linkageKeyPostRequest)
}
