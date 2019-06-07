package features.myrecord.factories

import mocking.data.myrecord.MyRecordSerenityHelpers
import mocking.microtest.myRecord.Allergy
import models.Patient
import utils.getOrFail
import worker.models.myrecord.AllergyItem

class AllergiesFactoryMicrotest: AllergiesFactory() {

    override fun disabled(patient: Patient) {
        throw UnsupportedOperationException()
    }

    override fun enabledWithRecords(patient: Patient, count: Int) {
        throw UnsupportedOperationException()
    }

    override fun getExpectedAllergies(): List<AllergyItem> {
        val allergies = MyRecordSerenityHelpers.ALLERGY_DATA.getOrFail<List<Allergy>>()

        return allergies.map {
            item -> AllergyItem(
                name = item.description,
                date = worker.models.myrecord.Date(
                        value = item.start_date,
                        datePart = item.start_date))
        }
    }
}
