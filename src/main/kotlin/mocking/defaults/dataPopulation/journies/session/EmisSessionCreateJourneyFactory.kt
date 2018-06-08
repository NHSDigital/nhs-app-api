package mocking.defaults.dataPopulation.journies.session

import mocking.MockingClient
import mocking.emis.models.AssociationType
import models.Patient

class EmisSessionCreateJourneyFactory(val client: MockingClient) {

    fun createFor(patient: Patient) {
        client
                .forEmis {
                    endUserSessionRequest()
                            .respondWithSuccess(patient.endUserSessionId)
                }

        client
                .forEmis {
                    sessionRequest(Patient(
                            endUserSessionId = patient.endUserSessionId,
                            connectionToken = patient.connectionToken,
                            odsCode = patient.odsCode)
                    )
                            .respondWithSuccess(patient, AssociationType.Self)
                }
    }
}