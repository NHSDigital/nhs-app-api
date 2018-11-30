package features.myrecord.factories

import mocking.data.myrecord.AllergiesData
import mocking.data.myrecord.ConsultationsData
import mocking.data.myrecord.ImmunisationsData
import mocking.data.myrecord.MedicationsData
import mocking.data.myrecord.ProblemsData
import mocking.data.myrecord.TestResultsData
import models.Patient

class MyRecordFactoryEmis: MyRecordFactory() {
    override fun disabled(patient: Patient) {

        mockingClient.forEmis {
            myRecord.allergiesRequest(patient).respondWithExceptionWhenNotEnabled()
        }
    }

    override fun enabledWithBlankRecord(patient: Patient) {
        mockingClient.forEmis {
            myRecord.testResultsRequest(patient)
                    .respondWithSuccess(TestResultsData.getDefaultTestResultsModel())
        }

        mockingClient.forEmis {
            myRecord.immunisationsRequest(patient)
                    .respondWithSuccess(ImmunisationsData.getDefaultImmunisationsModel())
        }

        mockingClient.forEmis {
            myRecord.allergiesRequest(patient).respondWithSuccess(AllergiesData.getEmisDefaultAllergyModel())
        }

        mockingClient.forEmis {
            myRecord.medicationsRequest(patient)
                    .respondWithSuccess(MedicationsData.getEmisDefaultMedicationsModel())
        }

        mockingClient.forEmis {
            myRecord.problemsRequest(patient).respondWithSuccess(ProblemsData.getDefaultProblemModel())
        }

        mockingClient.forEmis {
            myRecord.consultationsRequest(patient)
                    .respondWithSuccess(ConsultationsData.getDefaultConsultationsData())
        }
    }
}