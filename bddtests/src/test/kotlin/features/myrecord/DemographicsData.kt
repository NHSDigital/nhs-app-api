package features.myrecord

import mocking.emis.models.*
import worker.models.demographics.*

object DemographicsData {

    fun getDemographicData(): DemographicsResponse {

        val patientIdentifiers = mutableListOf<PatientIdentifier>()

        patientIdentifiers.add(PatientIdentifier("NHS123", IdentifierType.NhsNumber))

        return DemographicsResponse("Mr",
                "John",
                "Smith",
                "Johnny",

                patientIdentifiers,
                "1984-11-07T00:00:00",
                Sex.Male,
                ContactDetails("01011010101", "87878787878", "email@ddress.com"),
                Address("1", "2", "3", "4", "5", "6")
        )
    }
}
