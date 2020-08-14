package features.myrecord.factories

import mocking.data.myrecord.MyRecordSerenityHelpers
import mocking.microtest.myRecord.MyRecordResponseModel
import models.Patient
import utils.getOrFail
import worker.models.myrecord.ProblemItem
import worker.models.myrecord.ProblemLineItem

class ProblemsFactoryMicrotest: ProblemsFactory(){
    override fun badDataResponse(patient: Patient) {
            mockingClient.forMicrotest.mock {
                myRecord.myRecordRequest(patient)
                        .respondWithCorruptedContent("Bad Data")
            }
        }

    override fun getExpectedProblems(): List<ProblemItem> {
        val myRecord = MyRecordSerenityHelpers.MY_RECORD_DATA.getOrFail<MyRecordResponseModel>()
        val problems = myRecord.medicalProblems.data

        return problems.map { item ->
            ProblemItem(
                    effectiveDate = worker.models.myrecord.Date(
                            value = item.start_date,
                            datePart = "YearMonthDay"),
                    lineItems = mutableListOf(
                        ProblemLineItem(
                                text = "Finish Date: " + item.finish_date,
                                lineItems = mutableListOf()
                        ),
                        ProblemLineItem(
                                text = item.rubric,
                                lineItems = mutableListOf()
                        )
                    )
            )
        }.sortedByDescending { it.effectiveDate }.toList()
    }

    override fun secondProblemHasNoDate(patient: Patient) {
        throw UnsupportedOperationException("Not yet implemented")
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
