package features.myrecord.factories

import mocking.defaults.VisionMockDefaults
import mocking.stubs.StubbedEnvironment
import mocking.vision.models.VisionUserSession
import models.Patient
import utils.SerenityHelpers
import worker.NhsoHttpException
import java.time.Duration

class DemographicsFactoryVision: DemographicsFactory() {
    override fun disabled(patient: Patient) {
        try {
            mockingClient.forVision.mock {
                myRecord.demographicsRequest(visionUserSession = VisionUserSession.fromPatient(patient)
                ).respondWithAccessDeniedError()
            }
        } catch (httpException: NhsoHttpException) {
            SerenityHelpers.setHttpException(httpException)
        }
    }

    override fun enabled(patient: Patient) {
        mockingClient.forVision.mock {
            myRecord.demographicsRequest(visionUserSession =
            VisionUserSession.fromPatient(patient)).
                    respondWithSuccess(VisionMockDefaults.visionDemographicsResponse)

        }
    }

    override fun enabledViaProxy(callingPatient: Patient, actingOnBehalfOf: Patient) {
        throw NotImplementedError()
    }

    override fun enabledButTimesOut(patient: Patient) {
        mockingClient.forVision.mock {
            myRecord.demographicsRequest(visionUserSession =
            VisionUserSession.fromPatient(patient)).
                    respondWithSuccess(VisionMockDefaults.visionDemographicsResponse)
                    .delayedBy(Duration.ofSeconds(StubbedEnvironment.TIMEOUT_DELAY))
        }
    }

    override fun throwInternalError(patient: Patient) {
        mockingClient.forVision.mock {
            myRecord.demographicsRequest( VisionUserSession.fromPatient(patient))
                    .respondWithUnknownError()
        }
    }

    override fun throwForbiddenError(patient: Patient) {
        throw NotImplementedError()
    }

    override fun throwBadGateway(patient: Patient) {
        throw NotImplementedError()
    }
}
