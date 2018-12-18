package mocking.organDonation.models

import models.Patient

data class LookupRegistrationRequest(
        var identifier: String,
        var given: String,
        var family: String,
        var birthdate: String
)
{
    companion object {
        fun forPatient(patient: Patient): LookupRegistrationRequest {
            val nhsNumber = patient.nhsNumbers.first()
            return LookupRegistrationRequest(
                    identifier = "https://fhir.nhs.uk/Id/nhs-number|"
                            + nhsNumber,
                    given = patient.firstName,
                    family = patient.surname,
                    birthdate = patient.dateOfBirth
            )
        }
    }
}
