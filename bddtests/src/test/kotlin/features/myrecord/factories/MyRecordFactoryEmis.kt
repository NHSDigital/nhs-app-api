package features.myrecord.factories

import mocking.data.myrecord.AllergiesData
import mocking.data.myrecord.ConsultationsData
import mocking.data.myrecord.ImmunisationsData
import mocking.data.myrecord.MedicationsData
import mocking.data.myrecord.ProblemsData
import mocking.data.myrecord.TestResultsData
import models.Patient
import mocking.data.myrecord.NUMBER_OF_ALLERGY_RECORDS
import mocking.data.myrecord.NUMBER_OF_TEST_RESULT_RECORDS
import mocking.microtest.myRecord.MyRecordModuleCounts

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

    override fun enabledWithData(patient: Patient, myRecordModuleCounts: MyRecordModuleCounts) {
        throw UnsupportedOperationException()
    }

    override fun respondWithForbidden(patient: Patient) {
        throw UnsupportedOperationException()
    }

    override fun enabledWithAllRecords(patient: Patient) {
        mockingClient.forEmis {
            myRecord.testResultsRequest(patient)
                    .respondWithSuccess(TestResultsData.getTestResultsForEmis(NUMBER_OF_TEST_RESULT_RECORDS))
        }

        mockingClient.forEmis {
            myRecord.immunisationsRequest(patient)
                    .respondWithSuccess(ImmunisationsData.getImmunisationsData())
        }

        mockingClient.forEmis {
            myRecord.allergiesRequest(patient)
                    .respondWithSuccess(AllergiesData.getEmisAllergiesData(NUMBER_OF_ALLERGY_RECORDS))
        }

        mockingClient.forEmis {
            myRecord.medicationsRequest(patient)
                    .respondWithSuccess(MedicationsData.getEmisMedicationData())
        }

        mockingClient.forEmis {
            myRecord.problemsRequest(patient)
                    .respondWithSuccess(ProblemsData.getProblemsData())
        }

        mockingClient.forEmis {
            myRecord.consultationsRequest(patient)
                    .respondWithSuccess(ConsultationsData.getMultipleConsultationRecords())
        }
    }
}