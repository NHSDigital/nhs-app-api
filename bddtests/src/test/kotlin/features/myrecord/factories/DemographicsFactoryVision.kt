package features.myrecord.factories

import features.myrecord.stepDefinitions.HTTP_EXCEPTION
import mocking.stubs.StubbedEnvironment
import mocking.vision.VisionMockDefaults
import mocking.vision.models.VisionUserSession
import models.Patient
import net.serenitybdd.core.Serenity
import worker.NhsoHttpException
import java.time.Duration

class DemographicsFactoryVision: DemographicsFactory() {
    override fun disabled(patient: Patient) {
        try {
            mockingClient.forVision {
                myRecord.demographicsRequest(visionUserSession = VisionUserSession.fromPatient(patient)
                ).respondWithAccessDeniedError()
            }
        } catch (httpException: NhsoHttpException) {
            Serenity.setSessionVariable(HTTP_EXCEPTION).to(httpException)
        }
    }

    override fun enabled(patient: Patient) {
        mockingClient.forVision {
            myRecord.demographicsRequest(visionUserSession =
            VisionUserSession.fromPatient(patient)).
                    respondWithSuccess(VisionMockDefaults.visionDemographicsResponse)

        }
    }

    override fun enabledButTimesOut(patient: Patient) {
        mockingClient.forVision {
            myRecord.demographicsRequest(visionUserSession =
            VisionUserSession.fromPatient(patient)).
                    respondWithSuccess(VisionMockDefaults.visionDemographicsResponse)
                    .delayedBy(Duration.ofSeconds(StubbedEnvironment.TIMEOUT_DELAY))
        }
    }

    override fun throwInternalError(patient: Patient) {
        mockingClient.forVision {
            myRecord.demographicsRequest( VisionUserSession.fromPatient(patient))
                    .respondWithUnknownError()
        }
    }
}
