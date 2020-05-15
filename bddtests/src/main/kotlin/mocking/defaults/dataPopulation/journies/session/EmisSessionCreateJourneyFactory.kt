package mocking.defaults.dataPopulation.journies.session

import mocking.emis.models.AssociationType
import mocking.emis.practices.SettingsResponseModel
import models.Patient

class EmisSessionCreateJourneyFactory : SessionCreateJourneyFactory() {

    override fun createFor(patient: Patient, defaultPracticeSettings: Boolean) {
        createEndUserSessionRequest(patient)

        client.forEmis.mock {
            authentication.sessionRequest(patient).respondWithSuccess(patient, AssociationType.Self)
        }
        if (defaultPracticeSettings) {
            client.forEmis.mock {
                practiceSettingsRequest(patient)
                        .respondWithSuccess(SettingsResponseModel())
            }
        }
    }

    private fun createEndUserSessionRequest(patient: Patient) {
        client.forEmis.mock {
            authentication.endUserSessionRequest().respondWithSuccess(patient.endUserSessionId)
        }
    }
}
