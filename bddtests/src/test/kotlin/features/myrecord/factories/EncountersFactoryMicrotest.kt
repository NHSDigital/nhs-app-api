package features.myrecord.factories

import mocking.data.myrecord.MyRecordSerenityHelpers
import mocking.microtest.myRecord.MyRecordResponseModel
import utils.getOrFail
import worker.models.myrecord.EncounterItem

class EncountersFactoryMicrotest: EncountersFactory() {

    override fun getExpectedEncounters(): List<EncounterItem> {
        val myRecord = MyRecordSerenityHelpers.MY_RECORD_DATA.getOrFail<MyRecordResponseModel>()
        val encounters = myRecord.encounters.data

        val encounterItems = encounters.map { item ->
            EncounterItem(
                    recordedOn = worker.models.myrecord.Date(
                            value = item.recordedOn,
                            datePart = item.recordedOn),
                    description = item.description,
                    value = "Value: " + item.value,
                    unit = "Units: " + item.unit
            )
        }

        val sortedEncounters = encounterItems.sortedByDescending { it.recordedOn }

        return sortedEncounters
    }
}
