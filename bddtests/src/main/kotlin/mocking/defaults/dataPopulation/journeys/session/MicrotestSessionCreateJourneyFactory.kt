package mocking.defaults.dataPopulation.journeys.session

import models.Patient

class MicrotestSessionCreateJourneyFactory : SessionCreateJourneyFactory() {
    override fun createFor(patient: Patient, defaultPracticeSettings:Boolean) {
        client.forMicrotest.mock {
            demographics.demographicsRequest(patient).respondWithSuccess()
        }
    }
}
