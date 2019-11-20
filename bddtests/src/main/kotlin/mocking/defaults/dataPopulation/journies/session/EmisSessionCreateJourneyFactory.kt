package mocking.defaults.dataPopulation.journies.session

import mocking.MockingClient
import mocking.emis.models.AssociationType
import mocking.emis.practices.SettingsResponseModel
import models.Patient

class EmisSessionCreateJourneyFactory(val client: MockingClient) : SessionCreateJourneyFactory() {

    override fun createFor(patient: Patient, defaultPracticeSettings: Boolean) {
        createEndUserSessionRequest(patient)

        client.forEmis {
            authentication.sessionRequest(patient).respondWithSuccess(patient, AssociationType.Self)
        }
        if (defaultPracticeSettings) {
            client.forEmis {
                practiceSettingsRequest(patient)
                        .respondWithSuccess(SettingsResponseModel())
            }
        }
    }

    private fun createEndUserSessionRequest(patient: Patient) {
        client.forEmis {
            authentication.endUserSessionRequest().respondWithSuccess(patient.endUserSessionId)
        }
    }
}
