package features.myrecord.factories

import mocking.data.myrecord.MyRecordSerenityHelpers
import mocking.microtest.myRecord.MyRecordResponseModel
import models.Patient
import utils.getOrFail
import worker.models.myrecord.ProblemItem
import worker.models.myrecord.ProblemLineItem

class ProblemsFactoryMicrotest: ProblemsFactory(){

    override fun getExpectedProblems(): List<ProblemItem> {
        val myRecord = MyRecordSerenityHelpers.MY_RECORD_DATA.getOrFail<MyRecordResponseModel>()
        val problems = myRecord.medicalProblems.data

        return problems.map { item ->
            ProblemItem(
                    effectiveDate = worker.models.myrecord.Date(
                            value = item.start_date,
                            datePart = "Unknown"),
                    lineItems = mutableListOf(
                        ProblemLineItem(
                                text = "Finish Date: " + item.finish_date,
                                lineItems = mutableListOf<String>()
                        ),
                        ProblemLineItem(
                                text = item.rubric,
                                lineItems = mutableListOf<String>()
                        )
                    )
            )
        }
    }

    override fun errorRetrieving(patient: Patient) {
        throw UnsupportedOperationException()
    }

    override fun noAccess(patient: Patient) {
        throw UnsupportedOperationException()
    }

    override fun disabled(patient: Patient) {
        throw UnsupportedOperationException()
    }

    override fun enabledWithBlankRecord(patient: Patient) {
        throw UnsupportedOperationException()
    }

    override fun enabledWithRecords(patient: Patient) {
        throw UnsupportedOperationException()
    }
}