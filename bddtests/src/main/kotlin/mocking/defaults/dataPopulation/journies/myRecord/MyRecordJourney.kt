package mocking.dataPopulation.journies.myRecord

import mocking.MockingClient
import mocking.defaults.dataPopulation.journies.im1Connection.SuccessfulRegistrationJourney
import mocking.emis.demographics.EmisDemographicsResponse
import mocking.emis.demographics.PatientIdentifier
import mocking.emis.models.IdentifierType

class MyRecordJourney(private val client: MockingClient) {
    val patient = SuccessfulRegistrationJourney.patient
    
    fun create() {
        client.forEmis {
            demographicsRequest(patient)
                    .respondWithSuccess(
                            EmisDemographicsResponse(
                                    firstName = patient.firstName,
                                    surname = patient.surname,
                                    callingName = patient.callingName,
                                    patientIdentifiers = arrayListOf(
                                            PatientIdentifier(
                                                    identifierType = IdentifierType.NhsNumber,
                                                    identifierValue = patient.nhsNumbers[0]
                                            )
                                    ),
                                    dateOfBirth = patient.dateOfBirth,
                                    sex = patient.sex,
                                    contactDetails = patient.contactDetails,
                                    address = patient.address
                            )
                    )
        }
    }
}
