package features.authentication.factories

import mocking.vision.models.VisionUserSession

class Im1ConnectionV2GetFactoryVision : Im1ConnectionV2GetFactory("VISION") {

    override fun errorIm1Verify(httpStatusCode: Int, errorCode: String,
                                message: String?) {
        mockingClient
            .forVision {
                authentication.getConfigurationRequest(
                    visionUserSession = VisionUserSession(
                            patient.rosuAccountId,
                            patient.apiKey,
                            patient.odsCode,
                            patient.patientId
                    ))
                    .respondWithError(httpStatusCode, errorCode, message)
            }
    }
}
