package mocking.defaults.dataPopulation.journies.session

import mocking.MockingClient
import mocking.defaults.MockDefaults
import mocking.vision.VisionConstants.gpAppointmentsDisabled
import models.Patient
import net.serenitybdd.core.Serenity

class VisionSessionCreateJourneyFactory(val client: MockingClient): SessionCreateJourneyFactory() {

    override fun createFor(patient: Patient) {
        if(Serenity.sessionVariableCalled<String>(gpAppointmentsDisabled) == "true"){
            client
                    .forVision {
                        getConfigurationRequest(
                                MockDefaults.visionUserSession)
                                .respondWithSuccess(MockDefaults.visionConfigurationResponseAppointmentsDisabled)
                    }
        }
        else {
            client
                    .forVision {
                        getConfigurationRequest(MockDefaults.visionUserSession)
                                .respondWithSuccess(MockDefaults.visionConfigurationResponse)
                    }
        }
    }
}
