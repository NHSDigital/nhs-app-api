package mocking.defaults.dataPopulation.journies.session

import mocking.defaults.VisionMockDefaults
import mocking.vision.VisionConstants.gpAppointmentsDisabled
import mocking.vision.appointments.helpers.GeneralAppointmentsHelper
import mocking.vision.models.VisionUserSession
import mocking.vision.models.appointments.Location
import mocking.vision.models.appointments.Owner
import mocking.vision.models.appointments.References
import models.Patient
import net.serenitybdd.core.Serenity

class VisionSessionCreateJourneyFactory : SessionCreateJourneyFactory() {

    override fun createFor(patient: Patient, defaultPracticeSettings:Boolean) {
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

        client.forVision.mock {
                    authentication.getConfigurationRequest(
                            VisionUserSession.fromPatient(patient))
                            .respondWithSuccess(configuration)
                }
    }
}
