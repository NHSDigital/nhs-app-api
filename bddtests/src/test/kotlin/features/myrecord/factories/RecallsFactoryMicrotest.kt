package features.myrecord.factories

import mocking.data.myrecord.MyRecordSerenityHelpers
import mocking.microtest.myRecord.MyRecordResponseModel
import utils.SerenityHelpers
import utils.getOrFail
import worker.models.myrecord.RecallItem

class RecallsFactoryMicrotest: RecallsFactory(){

    override fun getExpectedRecalls(): List<RecallItem> {
        val myRecord = MyRecordSerenityHelpers.MY_RECORD_DATA.getOrFail<MyRecordResponseModel>()
        val recalls = myRecord.recalls.data

        val recallItems = recalls.map { item ->
            RecallItem(
                    recordDate = worker.models.myrecord.Date(
                            value = item.recordDate,
                            datePart = item.recordDate),
                    name =  item.name,
                    description = item.description,
                    result = "Result: " + item.result,
                    nextDate = "Next Date: " + item.nextDate,
                    status = "Status: " + item.status)
        }

        val sortedRecalls = recallItems.sortedByDescending{ it.recordDate }

        return sortedRecalls
    }

    override fun respondWithCorruptedResponse() {
        mockingClient.forMicrotest {
            myRecord.myRecordRequest(SerenityHelpers.getPatient())
                    .respondWithCorruptedContent("Bad Data")
        }
    }

}