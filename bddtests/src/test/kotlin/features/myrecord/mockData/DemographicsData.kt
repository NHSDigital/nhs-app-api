package features.myrecord.mockData

import mocking.emis.demographics.*
import mocking.emis.models.*
import mocking.tpp.models.*
import worker.models.demographics.*

object DemographicsData {

    fun getEmisDemographicData(): EmisDemographicsResponse {

        val patientIdentifiers = mutableListOf<PatientIdentifier>()

        patientIdentifiers.add(PatientIdentifier("NHS123", IdentifierType.NhsNumber))

        return EmisDemographicsResponse(
                "Mr",
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

    fun getTppDemographicsData(): PatientSelectedReply {

        val result = PatientSelectedReply(
                "84df400000000000",
                "84df400000000000",
                "1f907c07-9063-4d3a-81d7-ee8c98c54f4a",
                Person("84df400000000000",
                        "1985-05-29T00:00:00.0Z",
                        "Male", NationalId("0123456789"),
                        PersonName("Mr Kevin Barry"),
                        TppAddress("28 Central Path,  Troy Road, Horsforth, Leeds, West Yorkshire, LS18 5 TN"))
        )
        return result;
    }
}
