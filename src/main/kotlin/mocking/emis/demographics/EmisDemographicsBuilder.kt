package mocking.emis.demographics


import mocking.emis.*
import mocking.emis.models.*
import mocking.models.Mapping
import models.Patient
import org.apache.http.HttpStatus.SC_OK
import java.time.LocalDateTime

class EmisDemographicsBuilder(configuration: EmisConfiguration,
                              linkToken: String,
                              apiEndUserSessionId: String,
                              apiSessionId: String)
    : EmisMappingBuilder(configuration, "GET", "/demographics") {

    init {
        requestBuilder
                .andHeader(HEADER_API_END_USER_SESSION_ID, apiEndUserSessionId)
                .andHeader(HEADER_API_SESSION_ID, apiSessionId)
                .andQueryParameter("userPatientLinkToken", linkToken, "equalTo")
    }

    fun respondWithSuccess(patient: Patient, patientIdentifiers: Array<PatientIdentifier>): Mapping {
        val responseBody = DemographicsResponse(patient.title, patient.firstName, patient.surname, patientIdentifiers = patientIdentifiers.toMutableList())

        return respondWith(SC_OK) {
            andJsonBody(responseBody)
                    .build()
        }
    }

    fun respondWithSuccess(demographicResponse: DemographicsResponse): Mapping {
        return respondWith(SC_OK) {
            andJsonBody(demographicResponse)
                    .build()
        }
    }
}
