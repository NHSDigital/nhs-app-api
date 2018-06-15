package mocking.dataPopulation.journies.myRecord

import mocking.MockingClient
import mocking.defaults.dataPopulation.journies.im1Connection.SuccessfulRegistrationJourney
import mocking.emis.models.*
import worker.models.demographics.DemographicsResponse
import worker.models.demographics.PatientIdentifier

class MyRecordJourney(private val client: MockingClient) {
    val patient = SuccessfulRegistrationJourney.patient
    
    fun create() {
        client
                .forEmis {
                    demographicsRequest(patient)
                            .respondWithSuccess(
                                    DemographicsResponse(
                                            title = patient.title,
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
