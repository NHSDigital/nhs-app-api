package mocking.data.myrecord

import mocking.emis.allergies.AllergyMedicalRecord
import mocking.emis.allergies.AllergyResponse
import mocking.emis.allergies.AllergyResponseModel
import mocking.emis.allergies.EffectiveDate
import mocking.tpp.models.ViewPatientOverviewItem
import mocking.tpp.models.ViewPatientOverviewReply
import utils.set

const val NUMBER_OF_ALLERGY_RECORDS = 2

object AllergiesData {

    private const val TERM = "Hay Fever"
    private const val DATE_FOR_ALLERGY_RECORDS = "2018-05-15T09:52:44.927"

    fun getEmisAllergiesData(count: Int): AllergyResponseModel {
        val expectedAllergyData = arrayListOf(TERM)
        val expected = arrayListOf<String>()
        val allergies = mutableListOf<AllergyResponse>()

        for (i in 1..count) {
            allergies.add(AllergyResponse(term = TERM, effectiveDate =
                EffectiveDate("UnKnown", DATE_FOR_ALLERGY_RECORDS)))

            expected.add(expectedAllergyData[0])
        }

        MyRecordSerenityHelpers.EXPECTED_ALLERGY_DATA.set(expected)
        return AllergyResponseModel(
                medicalRecord = AllergyMedicalRecord(
                        allergies = allergies
                )
        )
    }

    fun getEmisAllergyRecordsWithDifferentDateParts(): AllergyResponseModel {
        val allergies = mutableListOf<AllergyResponse>()
        val allergyResponseData = arrayListOf(
                Pair("Hay Fever","UnKnown"),
                Pair("Nut Allergy","Year"),
                Pair("H/O: analgesic allergy","YearMonth"),
                Pair("H/O: penicillin allergy","YearMonthDay"),
                Pair("Hay Fever 2","YearMonthDayTime")
        )

        allergyResponseData.forEach { (allergyResponseTerm,allergyResponseDatePart)-> allergies.add(
            AllergyResponse(
                    term = allergyResponseTerm,
                    effectiveDate = EffectiveDate(allergyResponseDatePart, DATE_FOR_ALLERGY_RECORDS)
            ))
        }

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
        val expectedAllergyData = arrayListOf(TERM)
        val expected = arrayListOf<String>()
        val allergies = mutableListOf<ViewPatientOverviewItem>()
        val drugSensitivities = mutableListOf<ViewPatientOverviewItem>()

        for (i in 1..count) {
            allergies.add(ViewPatientOverviewItem(id = i.toString(), description = "Allergies",
                                                  date=DATE_FOR_ALLERGY_RECORDS, value=TERM))

            expected.add(expectedAllergyData[0])
        }

        MyRecordSerenityHelpers.EXPECTED_ALLERGY_DATA.set(expected)
        return ViewPatientOverviewReply(
                drugSensitivities = drugSensitivities,
                allergies = allergies
        )
    }

    fun getVisionAllergiesData(count: Int): String {
        val expectedAllergyData = arrayListOf("Hay Fever", "Paracetamol 500mg capsules", "Leg swelling")
        val expected = arrayListOf<String>()

        val allergy = "<clinical eventdate=\"2007-05-12T00:00:00\" drug_term=\"Paracetamol" +
                " 500mg capsules\" read_code=\"14L..00\" read_term=\"Hay Fever\" read_code2=\"1833" +
                ".00\" read_term2=\"Leg swelling\"/>"

        var response = "<![CDATA[<root><patient>"
        val responseStringEnd = "</patient></root>]]>"

        for(i in 1..count) {
            response += allergy
            expected.addAll(expectedAllergyData)
        }
        MyRecordSerenityHelpers.EXPECTED_ALLERGY_DATA.set(expected)
        return response + responseStringEnd
    }

    fun getVisionAllergiesDrugAndNonDrugData(): String {
        return "<![CDATA[<root><patient><clinical eventdate=\"2007-05-12T00:00:00\" drug_term=\"Paracetamol 500mg " +
            "capsules\" read_code=\"14L..00\" read_term=\"H/O: drug allergy\" read_code2=\"1833.00\" read_term2=\"Leg" +
            " swelling\"/><clinical eventdate=\"2007-05-12T00:00:00\" read_term=\"Pollen\"/></patient></root>]]>"
    }
}
