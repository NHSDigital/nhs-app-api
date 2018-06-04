package features.myrecord

import mocking.emis.models.*

object AllergiesData {

    fun getAllergiesData(): AllergiesResponse {

        val allergies = mutableListOf<AllergyItem>()
        allergies.add(AllergyItem(term = "Hay Fever", availabilityDateTime = "2018-05-15T09:52:44.927"))
        allergies.add(AllergyItem(term = "H/O: rotavirus vaccine allergy", availabilityDateTime = "2018-05-14T09:52:44.927"))

        var result = AllergiesResponse (
                medicalRecord = AllergyMedicalRecord (
                        allergies = allergies
                )
        )

        return result;
    }
}