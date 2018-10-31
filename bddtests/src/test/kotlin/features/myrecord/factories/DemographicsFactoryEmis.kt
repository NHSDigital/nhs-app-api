package features.myrecord.factories

import features.myrecord.stepDefinitions.HTTP_EXCEPTION
import mocking.data.myrecord.DemographicsData
import models.Patient
import net.serenitybdd.core.Serenity
import worker.NhsoHttpException

class DemographicsFactoryEmis: DemographicsFactory() {
    override fun disabledFunctionality(patient: Patient) {
        try {
            mockingClient.forEmis {
                myRecord.demographicsRequest(patient).respondWithExceptionWhenNotEnabled()
            }
        } catch (httpException: NhsoHttpException) {
            Serenity.setSessionVariable(HTTP_EXCEPTION).to(httpException)
        }
    }

    override fun enabledFunctionality(patient: Patient) {
        mockingClient.forEmis {
            myRecord.demographicsRequest(patient)
                    .respondWithSuccess(DemographicsData.getEmisDemographicData(patient))
        }
    }

}