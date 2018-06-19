package features.myrecord

import mocking.emis.allergies.AllergyMedicalRecord
import mocking.emis.allergies.AllergyResponse
import mocking.emis.allergies.AllergyResponseModel
import mocking.emis.allergies.EffectiveDate

object AllergiesData {

    val TERM = "Hay Fever"
    val DATE = "2018-05-15T09:52:44.927"

    fun getAllergiesData(count: Int): AllergyResponseModel {

        val allergies = mutableListOf<AllergyResponse>()

        for (i in 1..count) {
            allergies.add(AllergyResponse(term = TERM, effectiveDate = EffectiveDate("UnKnown", DATE)))
        }

        return AllergyResponseModel(
                medicalRecord = AllergyMedicalRecord(
                        allergies = allergies
                )
        )
    }

    fun getAllergyRecordsWithDifferentDateParts(): AllergyResponseModel {

        val allergies = mutableListOf<AllergyResponse>()

        allergies.add(AllergyResponse(term = "Hay Fever", effectiveDate = EffectiveDate("UnKnown", "2018-05-15T09:52:44.927")))
        allergies.add(AllergyResponse(term = "Nut Allergy", effectiveDate = EffectiveDate("Year", "2018-05-15T09:52:44.927")))
        allergies.add(AllergyResponse(term = "H/O: analgesic allergy", effectiveDate = EffectiveDate("YearMonth", "2018-05-15T09:52:44.927")))
        allergies.add(AllergyResponse(term = "H/O: penicillin allergy", effectiveDate = EffectiveDate("YearMonthDay", "2018-05-15T09:52:44.927")))
        allergies.add(AllergyResponse(term = "Hay Fever 2", effectiveDate = EffectiveDate("YearMonthDayTime", "2018-05-15T09:52:44.927")))

        return AllergyResponseModel(
                medicalRecord = AllergyMedicalRecord(
                        allergies = allergies
                )
        )
    }
}