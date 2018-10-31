package features.myrecord.factories

import features.myrecord.stepDefinitions.HTTP_EXCEPTION
import mocking.vision.VisionMockDefaults
import mocking.vision.models.VisionUserSession
import models.Patient
import net.serenitybdd.core.Serenity
import worker.NhsoHttpException

class DemographicsFactoryVision: DemographicsFactory() {
    override fun disabledFunctionality(patient: Patient) {
        try {
            mockingClient.forVision {
                demographicsRequest(visionUserSession = VisionUserSession.fromPatient(patient)
                ).respondWithAccessDeniedError()
            }
        } catch (httpException: NhsoHttpException) {
            Serenity.setSessionVariable(HTTP_EXCEPTION).to(httpException)
        }
    }

    override fun enabledFunctionality(patient: Patient) {
        mockingClient.forVision {
            demographicsRequest(visionUserSession =
            VisionUserSession.fromPatient(patient)).
                    respondWithSuccess(VisionMockDefaults.visionDemographicsResponse)

        }
    }
}