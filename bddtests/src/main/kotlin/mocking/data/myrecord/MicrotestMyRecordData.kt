package mocking.data.myrecord

import mocking.microtest.myRecord.Allergies
import mocking.microtest.myRecord.Allergy
import mocking.microtest.myRecord.MyRecordResponseModel
import utils.set

object MicrotestMyRecordData {

    fun getEmptyMicrotestMyRecord(): MyRecordResponseModel {

        val allergies = Allergies("true", "false", 0, mutableListOf<Allergy>())
        return MyRecordResponseModel(allergies)
    }

    fun getPopulatedMicrotestMyRecord(numAllergies: Int): MyRecordResponseModel {

        val allergyList = mutableListOf<Allergy>()

        for (i in 1..numAllergies) {
            allergyList.add(
                    Allergy(
                            id = "$i",
                            type = "Drug",
                            start_date = "2019-03-27",
                            description = "Penicilin $i",
                            severity = "Low"
                    )
            )
        }

        MyRecordSerenityHelpers.ALLERGY_DATA.set(allergyList)

        val allergies = Allergies("true", "false", allergyList.size, allergyList)

        return MyRecordResponseModel(allergies)
    }

}
