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

        allergies.add(AllergyResponse(term = "Hay Fever", effectiveDate = EffectiveDate(
                "UnKnown", "2018-05-15T09:52:44.927")))
        allergies.add(AllergyResponse(term = "Nut Allergy", effectiveDate = EffectiveDate(
                "Year", "2018-05-15T09:52:44.927")))
        allergies.add(AllergyResponse(term = "H/O: analgesic allergy", effectiveDate = EffectiveDate(
                "YearMonth", "2018-05-15T09:52:44.927")))
        allergies.add(AllergyResponse(term = "H/O: penicillin allergy", effectiveDate = EffectiveDate(
                "YearMonthDay", "2018-05-15T09:52:44.927")))
        allergies.add(AllergyResponse(term = "Hay Fever 2", effectiveDate = EffectiveDate(
                "YearMonthDayTime", "2018-05-15T09:52:44.927")))

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
            allergies.add(ViewPatientOverviewItem(id = i.toString(), description = "Allergies",
                                                  date="2018-05-15T09:52:44.927", value="Hay Fever"))
        }

        val result= ViewPatientOverviewReply(
                drugSensitivities = drugSensitivities,
                allergies = allergies
        )
        return result
    }

    fun getVisionAllergiesData(count: Int): String {
        val allergy = "<clinical eventdate=\"2007-05-12T00:00:00\" drug_term=\"Paracetamol" +
                " 500mg capsules\" read_code=\"14L..00\" read_term=\"Hay Fever\" read_code2=\"1833" +
                ".00\" read_term2=\"Leg swelling\"/>"

        var response = "<![CDATA[<root><patient>"
        val responseStringEnd = "</patient></root>]]>"

        for(i in 1..count) {
            response += allergy
        }

        return response + responseStringEnd
    }

    fun getVisionAllergiesDrugAndNonDrugData(): String {
        return "<![CDATA[<root><patient><clinical eventdate=\"2007-05-12T00:00:00\" drug_term=\"Paracetamol 500mg " +
            "capsules\" read_code=\"14L..00\" read_term=\"H/O: drug allergy\" read_code2=\"1833.00\" read_term2=\"Leg" +
            " swelling\"/><clinical eventdate=\"2007-05-12T00:00:00\" read_term=\"Pollen\"/></patient></root>]]>"
    }
}
