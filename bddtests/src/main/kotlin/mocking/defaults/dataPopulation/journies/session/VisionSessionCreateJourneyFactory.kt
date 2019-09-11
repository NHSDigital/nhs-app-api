package mocking.defaults.dataPopulation.journies.session

import mocking.MockingClient
import mocking.vision.VisionConstants.gpAppointmentsDisabled
import mocking.defaults.VisionMockDefaults
import mocking.vision.appointments.helpers.GeneralAppointmentsHelper
import mocking.vision.models.VisionUserSession
import mocking.vision.models.appointments.Location
import mocking.vision.models.appointments.Owner
import mocking.vision.models.appointments.References
import models.Patient
import net.serenitybdd.core.Serenity

class VisionSessionCreateJourneyFactory(val client: MockingClient) : SessionCreateJourneyFactory() {

    override fun createForWithLinkedAccounts(patient: Patient) {
        throw UnsupportedOperationException()
    }

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
                            VisionUserSession.fromPatient(patient))
                            .respondWithSuccess(configuration)
                }
    }
}
