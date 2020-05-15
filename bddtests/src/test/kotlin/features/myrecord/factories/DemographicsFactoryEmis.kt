package features.myrecord.factories

import mocking.data.myrecord.DemographicsData
import mocking.stubs.StubbedEnvironment
import models.Patient
import org.apache.http.HttpStatus
import utils.SerenityHelpers
import worker.NhsoHttpException
import java.time.Duration

class DemographicsFactoryEmis: DemographicsFactory() {
    override fun disabled(patient: Patient) {
        try {
            mockingClient.forEmis.mock {
                myRecord.demographicsRequest(patient).respondWithExceptionWhenNotEnabled()
            }
        } catch (httpException: NhsoHttpException) {
            SerenityHelpers.setHttpException(httpException)
        }
    }

    override fun enabled(patient: Patient) {
        mockingClient.forEmis.mock {
            myRecord.demographicsRequest(patient)
                    .respondWithSuccess(DemographicsData.getEmisDemographicData(patient))
        }
    }

    override fun enabledViaProxy(callingPatient: Patient, actingOnBehalfOf: Patient) {
        mockingClient.forEmis.mock {
            myRecord.demographicsRequest(
                    actingOnBehalfOf,
                    sessionId = callingPatient.sessionId,
                    endUserSessionId = callingPatient.endUserSessionId
            )
                    .respondWithSuccess(DemographicsData.getEmisDemographicData(actingOnBehalfOf))
        }
    }

    override fun enabledButTimesOut(patient: Patient) {
        mockingClient.forEmis.mock {
            myRecord.demographicsRequest(patient)
                    .respondWithSuccess(DemographicsData.getEmisDemographicData(patient)).delayedBy(Duration.ofSeconds
            (StubbedEnvironment.TIMEOUT_DELAY))
        }
    }

    override fun throwInternalError(patient: Patient) {
        mockingClient.forEmis.mock {
            myRecord.demographicsRequest(patient)
                    .respondWithStandardError(HttpStatus.SC_INTERNAL_SERVER_ERROR,
                            HttpStatus.SC_INTERNAL_SERVER_ERROR)
        }
    }

    override fun throwForbiddenError(patient: Patient) {
        mockingClient.forEmis.mock {
            myRecord.demographicsRequest(patient)
                    .respondWithStandardError(HttpStatus.SC_FORBIDDEN,
                            HttpStatus.SC_FORBIDDEN)
        }
    }

    override fun throwBadGateway(patient: Patient) {
        mockingClient.forEmis.mock {
            myRecord.demographicsRequest(patient)
                    .respondWithStandardError(HttpStatus.SC_BAD_GATEWAY,
                            HttpStatus.SC_BAD_GATEWAY)
        }
    }
}
