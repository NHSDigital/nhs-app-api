package features.myrecord.factories

import mocking.data.myrecord.MyRecordSerenityHelpers
import mocking.microtest.myRecord.MyRecordResponseModel
import models.Patient
import utils.getOrFail
import worker.models.myrecord.ImmunisationItem

class ImmunisationsFactoryMicrotest: ImmunisationsFactory(){

    override fun enabledWithBlankRecord(patient: Patient) {
        throw UnsupportedOperationException()
    }

    override fun enabledWithRecords(patient: Patient) {
        throw UnsupportedOperationException()
    }

    override fun getExpectedImmunisations(): List<ImmunisationItem> {

        val myRecord = MyRecordSerenityHelpers.MY_RECORD_DATA.getOrFail<MyRecordResponseModel>()
        val immunisations = myRecord.vaccinations.data

        return immunisations.map { item ->
            ImmunisationItem(
                    term = item.description,
                    effectiveDate = worker.models.myrecord.Date(
                            value = item.date,
                            datePart = item.date),
                    nextDate = "Next Date: " + item.nextDate,
                    status = "Status: " + item.status)
        }
    }

    override fun errorRetrieving(patient: Patient) {
        throw UnsupportedOperationException()
    }

    override fun noAccess(patient: Patient) {
        throw UnsupportedOperationException()
    }

}