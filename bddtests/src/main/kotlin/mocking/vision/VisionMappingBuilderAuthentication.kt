package mocking.vision

import mocking.MappingBuilder
import mocking.defaults.VisionMockDefaults
import mocking.vision.models.VisionUserSession
import models.Patient

open class VisionMappingBuilderAuthentication(method: String = "POST") : MappingBuilder(method, "/vision/") {

    fun getConfigurationRequest(visionUserSession: VisionUserSession): VisionGetConfigurationBuilder {
        return VisionGetConfigurationBuilder(visionUserSession,
                VisionMockDefaults.visionGetConfiguration)
    }

    fun getRegisterRequest(visionUserSession: VisionUserSession, patient: Patient):VisionRegisterBuilder {
        return VisionRegisterBuilder(visionUserSession, VisionMockDefaults.visionGetRegister, patient)
    }
}
