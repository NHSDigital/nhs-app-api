package features.myrecord.factories

import mocking.data.myrecord.MyRecordSerenityHelpers
import mocking.microtest.myRecord.MyRecordResponseModel
import models.Patient
import utils.getOrFail
import worker.models.myrecord.MedicalHistoryItem

class MedicalHistoryFactoryMicrotest: MedicalHistoryFactory(){

    override fun enabledWithBlankRecord(patient: Patient) {
        throw UnsupportedOperationException()
    }

    override fun enabledWithRecords(patient: Patient) {
        throw UnsupportedOperationException()
    }

    override fun getExpectedMedicalHistory(): List<MedicalHistoryItem> {
        val myRecord = MyRecordSerenityHelpers.MY_RECORD_DATA.getOrFail<MyRecordResponseModel>()
        val medicalHistories = myRecord.medicalHistory.data

        return medicalHistories.map { item ->
            MedicalHistoryItem(
                    startDate = worker.models.myrecord.Date(
                            value = item.start_date,
                            datePart = item.start_date),
                    rubric = "Rubric: " + item.rubric,
                    description = "Description: "+item.description)
        }
    }

}