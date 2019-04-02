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
        mockingClient.forMicrotest {
            demographics.demographicsRequest(patient).respondWithSuccess()
        }
    }

    override fun enabledButTimesOut(patient: Patient) {
        mockingClient.forMicrotest {
            demographics.demographicsRequest(patient)
                    .respondWithSuccess()
                    .delayedBy(Duration.ofSeconds(StubbedEnvironment.TIMEOUT_DELAY))
        }
    }

    override fun throwInternalError(patient: Patient) {
        mockingClient.forMicrotest {
            demographics.demographicsRequest(patient)
                    .respondWithInternalServerError()
        }
    }
}
