package features.myrecord.factories

import mocking.data.myrecord.MyRecordSerenityHelpers
import mocking.tpp.models.ViewPatientOverviewReply
import java.time.LocalDateTime
import mocking.tpp.models.ViewPatientOverviewItem
import mocking.data.myrecord.NUMBER_OF_ALLERGY_RECORDS
import utils.set

private const val NUMBER_OF_REPEAT_MEDICATIONS = 3
private const val NUMBER_OF_DRUGS = 1

class PatientOverviewFactoryTpp {

    private val now = LocalDateTime.now()
    private val tenMonthsAgo = now.minusMonths(TEN_MONTHS).toString()
    private val twentyMonthsAgo = now.minusMonths(TWENTY_MONTHS).toString()

    fun getTppPatientOverviewData(): ViewPatientOverviewReply {

        return ViewPatientOverviewReply(
                drugs = generateMedicationsList(NUMBER_OF_DRUGS,tenMonthsAgo),
                currentRepeats = generateMedicationsList(NUMBER_OF_REPEAT_MEDICATIONS,tenMonthsAgo),
                pastRepeats = generateMedicationsList(NUMBER_OF_REPEAT_MEDICATIONS,twentyMonthsAgo),
                allergies = generateAllergyList(NUMBER_OF_ALLERGY_RECORDS)
        )
    }

    private fun generateMedicationsList(count:Int, date:String): MutableList<ViewPatientOverviewItem>{
        val medicationList = mutableListOf<ViewPatientOverviewItem>()
        val medicineName = arrayListOf("Penecillin","Ventolin","Salbutamol","Calpol","Amoxycillin","Ibuprofen")
        for (i in 1..count){
            medicationList.add(ViewPatientOverviewItem(
                    date = date,
                    value = medicineName[i]))
        }
       return medicationList

    }

    private fun generateAllergyList(count:Int): MutableList<ViewPatientOverviewItem> {
        val expectedAllergyData = arrayListOf("Hay Fever")
        val expected = arrayListOf<String>()

        val allergies = mutableListOf<ViewPatientOverviewItem>()
        for (i in 1..count) {
            allergies.add(ViewPatientOverviewItem(
                    id = i.toString(),
                    description = "Allergies",
                    date = "2018-05-15T09:52:44.927",
                    value = "Hay Fever"))

            expected.add(expectedAllergyData[0])
        }
        MyRecordSerenityHelpers.EXPECTED_ALLERGY_DATA.set(expected)
        return allergies
    }
}
