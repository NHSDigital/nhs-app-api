package mocking.data.myrecord

import mocking.emis.allergies.AllergyMedicalRecord
import mocking.emis.allergies.AllergyResponse
import mocking.emis.allergies.AllergyResponseModel
import mocking.emis.allergies.EffectiveDate
import mocking.tpp.models.ViewPatientOverviewItem
import mocking.tpp.models.ViewPatientOverviewReply

object AllergiesData {

    val TERM = "Hay Fever"
    val DATE = "2018-05-15T09:52:44.927"

    fun getEmisAllergiesData(count: Int): AllergyResponseModel {

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

    fun getEmisAllergyRecordsWithDifferentDateParts(): AllergyResponseModel {

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


    fun getEmisDefaultAllergyModel(): AllergyResponseModel {
        return AllergyResponseModel(
                medicalRecord = AllergyMedicalRecord(
                        allergies = mutableListOf()
                )
        )
    }

    fun getTppAllergiesData(count: Int): ViewPatientOverviewReply {

        val allergies = mutableListOf<ViewPatientOverviewItem>()
        val drugSensitivities = mutableListOf<ViewPatientOverviewItem>()

        for (i in 1..count) {
            allergies.add(ViewPatientOverviewItem(id = i.toString(), description = "Allergies", date="2018-05-15T09:52:44.927", value="Hay Fever"))
        }

        val result= ViewPatientOverviewReply(
                drugSensitivities = drugSensitivities,
                allergies = allergies
        );
        return result;
    }

    fun getTppDefaultAllergyModel(): ViewPatientOverviewReply {
        return ViewPatientOverviewReply(
                allergies = mutableListOf<ViewPatientOverviewItem>(),
                drugSensitivities = mutableListOf<ViewPatientOverviewItem>()
        );
    }
}
