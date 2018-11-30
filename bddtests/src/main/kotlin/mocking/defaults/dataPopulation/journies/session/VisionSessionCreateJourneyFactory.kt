package mocking.defaults.dataPopulation.journies.session

import mocking.MockingClient
import mocking.vision.VisionConstants.gpAppointmentsDisabled
import mocking.vision.VisionMockDefaults
import mocking.vision.appointments.helpers.GeneralAppointmentsHelper
import mocking.vision.models.appointments.Location
import mocking.vision.models.appointments.Owner
import mocking.vision.models.appointments.References
import models.Patient
import net.serenitybdd.core.Serenity

class VisionSessionCreateJourneyFactory(val client: MockingClient) : SessionCreateJourneyFactory() {

    override fun createFor(patient: Patient) {
        var configuration = if (Serenity.sessionVariableCalled<String>(gpAppointmentsDisabled) == "true") {
            VisionMockDefaults.visionConfigurationResponseAppointmentsDisabled
        } else {
            VisionMockDefaults.visionConfigurationResponse
        }

        val locationsForPatient = Serenity.sessionVariableCalled<List<Location>>(
                GeneralAppointmentsHelper.Companion.VisionMetadata.LOCATIONS
        )
        val ownersForPatient = Serenity.sessionVariableCalled<List<Owner>>(
                GeneralAppointmentsHelper.Companion.VisionMetadata.OWNERS
        )
        configuration = configuration.copy(references =
                References(locationsForPatient, ownersForPatient)
        )

        client
                .forVision {
                    authentication.getConfigurationRequest(
                            VisionMockDefaults.visionUserSession)
                            .respondWithSuccess(configuration)
                }
    }
}
