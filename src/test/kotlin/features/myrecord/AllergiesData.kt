package features.myrecord

import mocking.emis.allergies.AllergyMedicalRecord
import mocking.emis.allergies.AllergyResponse
import mocking.emis.allergies.AllergyResponseModel
import mocking.emis.allergies.EffectiveDate

object AllergiesData {

    fun getAllergiesData(count: Int): AllergyResponseModel {

        val allergies = mutableListOf<AllergyResponse>()

        for (i in 1..count) {
            allergies.add(AllergyResponse(term = "Hay Fever", effectiveDate = EffectiveDate("UnKnown", "2018-05-15T09:52:44.927")))
        }

        return AllergyResponseModel (
                medicalRecord = AllergyMedicalRecord (
                        allergies = allergies
                )
        )
    }
}