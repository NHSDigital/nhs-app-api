package features.myrecord.factories

import mocking.stubs.StubbedEnvironment
import models.Patient
import org.junit.Assert
import java.time.Duration

class DemographicsFactoryMicrotest: DemographicsFactory() {
    override fun disabled(patient: Patient) {
        Assert.fail("Method is not yet set up for Microtest")
    }

    override fun enabled(patient: Patient) {
        mockingClient.forMicrotest.mock {
            demographics.demographicsRequest(patient).respondWithSuccess()
        }
    }

    override fun enabledViaProxy(callingPatient: Patient, actingOnBehalfOf: Patient) {
        throw NotImplementedError()
    }

    override fun enabledButTimesOut(patient: Patient) {
        mockingClient.forMicrotest.mock {
            demographics.demographicsRequest(patient)
                    .respondWithSuccess()
                    .delayedBy(Duration.ofSeconds(StubbedEnvironment.TIMEOUT_DELAY))
        }
    }

    override fun throwInternalError(patient: Patient) {
        mockingClient.forMicrotest.mock {
            demographics.demographicsRequest(patient)
                    .respondWithInternalServerError()
        }
    }

    override fun throwForbiddenError(patient: Patient) {
        mockingClient.forMicrotest.mock {
            demographics.demographicsRequest(patient)
                    .respondWithForbiddenError()
        }
    }

    override fun throwBadGateway(patient: Patient) {
        mockingClient.forMicrotest.mock {
            demographics.demographicsRequest(patient)
                    .respondWithBadGateway()
        }
    }
}
