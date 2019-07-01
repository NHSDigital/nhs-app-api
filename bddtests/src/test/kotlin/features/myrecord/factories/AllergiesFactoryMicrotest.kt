package features.myrecord.factories

import mocking.data.myrecord.MyRecordSerenityHelpers
import mocking.microtest.myRecord.MyRecordResponseModel
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

        val myRecord = MyRecordSerenityHelpers.MY_RECORD_DATA.getOrFail<MyRecordResponseModel>()
        val allergies = myRecord.allergies.data

        return allergies.map {
            item -> AllergyItem(
                name = item.description,
                date = worker.models.myrecord.Date(
                        value = item.start_date,
                        datePart = item.start_date))
        }
    }
}
