package features.myrecord.factories

import features.myrecord.stepDefinitions.HTTP_EXCEPTION
import mocking.data.myrecord.DemographicsData
import mocking.tpp.models.Error
import models.Patient
import net.serenitybdd.core.Serenity
import worker.NhsoHttpException

class DemographicsFactoryTpp: DemographicsFactory() {

    override fun disabled(patient: Patient) {
        try {
            mockingClient.forTpp {
                myRecord.patientSelectedPost(patient.tppUserSession!!)
                        .respondWithError(Error("6", "Error Occurred", "1f907c07-9063-4d3a-81d7-ee8c98c54f4a"))
            }
        } catch (httpException: NhsoHttpException) {
            Serenity.setSessionVariable(HTTP_EXCEPTION).to(httpException)
        }
    }

    override fun enabled(patient: Patient) {
        mockingClient.forTpp {
            myRecord.patientSelectedPost(patient.tppUserSession!!)
                    .respondWithSuccess(DemographicsData.getTppDemographicsData(patient))
        }
    }
}