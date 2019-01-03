package features.myrecord.factories

import features.myrecord.stepDefinitions.HTTP_EXCEPTION
import mocking.data.myrecord.DemographicsData
import mocking.stubs.StubbedEnvironment
import models.Patient
import net.serenitybdd.core.Serenity
import worker.NhsoHttpException
import java.time.Duration

class DemographicsFactoryEmis: DemographicsFactory() {
    override fun disabled(patient: Patient) {
        try {
            mockingClient.forEmis {
                myRecord.demographicsRequest(patient).respondWithExceptionWhenNotEnabled()
            }
        } catch (httpException: NhsoHttpException) {
            Serenity.setSessionVariable(HTTP_EXCEPTION).to(httpException)
        }
    }

    override fun enabled(patient: Patient) {
        mockingClient.forEmis {
            myRecord.demographicsRequest(patient)
                    .respondWithSuccess(DemographicsData.getEmisDemographicData(patient))
        }
    }

    override fun enabledButTimesOut(patient: Patient) {
        mockingClient.forEmis {
            myRecord.demographicsRequest(patient)
                    .respondWithSuccess(DemographicsData.getEmisDemographicData(patient)).delayedBy(Duration.ofSeconds
            (StubbedEnvironment.TIMEOUT_DELAY))
        }
    }
}
