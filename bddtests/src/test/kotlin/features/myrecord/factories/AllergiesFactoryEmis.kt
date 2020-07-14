package features.myrecord.factories

import mocking.data.myrecord.AllergiesData
import models.Patient
import worker.models.myrecord.AllergyItem
import worker.models.myrecord.Date

class AllergiesFactoryEmis: AllergiesFactory() {

    override fun disabled(patient: Patient) {
        mockingClient.forEmis.mock {
            myRecord.allergiesRequest(patient)
                    .respondWithExceptionWhenNotEnabled()
        }
    }

    override fun enabledWithRecords(patient: Patient, count: Int) {
        mockingClient.forEmis.mock {
            myRecord.allergiesRequest(patient)
                    .respondWithSuccess(AllergiesData.getEmisAllergiesData(count))
        }
    }

    override fun getExpectedAllergies(): List<AllergyItem> {
        return AllergiesData.getEmisAllergiesData(2).medicalRecord.allergies.map {
            element -> AllergyItem(element.term, Date(element.effectiveDate?.value.toString(),
                element.effectiveDate?.datePart.toString()))
        }.sortedByDescending { it.date }.toList()
    }

    override fun respondWithCorruptedContent(patient: Patient) {
        mockingClient.forEmis.mock {
            myRecord.allergiesRequest(patient)
                    .respondWithCorruptedContent()
        }
    }
}
