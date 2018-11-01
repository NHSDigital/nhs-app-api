package mocking.defaults.dataPopulation.journies.session

import mocking.MockingClient
import mocking.defaults.MockDefaults
import mocking.vision.VisionConstants.gpAppointmentsDisabled
import mocking.vision.appointments.helpers.GeneralAppointmentsHelper
import mocking.vision.models.appointments.Appointments
import mocking.vision.models.appointments.Location
import mocking.vision.models.appointments.Owner
import mocking.vision.models.appointments.References
import models.Patient
import net.serenitybdd.core.Serenity

class VisionSessionCreateJourneyFactory(val client: MockingClient) : SessionCreateJourneyFactory() {

    override fun createFor(patient: Patient) {
        var configuration = if (Serenity.sessionVariableCalled<String>(gpAppointmentsDisabled) == "true") {
            MockDefaults.visionConfigurationResponseAppointmentsDisabled
        } else {
            MockDefaults.visionConfigurationResponse
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
                    getConfigurationRequest(
                            MockDefaults.visionUserSession)
                            .respondWithSuccess(configuration)
                }
    }
}
